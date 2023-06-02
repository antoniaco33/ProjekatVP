using Common;
using Common.Models;
using DataBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service
{
    public class FileTransportService : IFileTransport
    {
        public static Database db = new Database();
        public Calculation calculation = new Calculation();

        public FileManipulationOptions GetCalculations(string command)
        {
            List<Load> loads = db.Read(ConfigurationManager.AppSettings["DBLoads"]);
            List<double> measuredValues = new List<double>();
            foreach (Load load in loads)
            {
                measuredValues.Add(load.MeasuredValue);
            }

            List<double> calcutaions = calculation.ProcessData(command, measuredValues);

            var options = new FileManipulationOptions();
            options.FileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".txt";

            var stringBuilder = new StringBuilder();

            foreach (var number in calcutaions)
            {
                stringBuilder.AppendLine("Vrednost: " + number);
            }

            var bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());

            options.MS = new MemoryStream(bytes);

            return options;
        }

       
        [OperationBehavior(AutoDisposeParameters = true)]
        public bool ParseFile(FileManipulationOptions options, out List<Audit> errors)
        {
            errors = new List<Audit>();
            List<Load> values = new List<Load>();
            int line = 1;

            using (StreamReader stream = new StreamReader(options.MS))
            {
                string data = stream.ReadToEnd();
                string[] csv_rows = data.Split('\n');
                string[] rows = csv_rows.Take(csv_rows.Length).ToArray();

                foreach (var row in rows)
                {
                    string[] rowSplit = row.Split(',');

                    if (rowSplit.Length != 3)
                    {
                        errors.Add(
                                new Audit(0, DateTime.Now, MessageType.Error, "Invalid data format in CSV file " + DateTime.Now.ToString("yyyy-MM-dd HH-mm"))
                            );
                    }
                    else
                    {
                        if (!DateTime.TryParse(rowSplit[1], out DateTime vreme))
                        {
                            errors.Add(
                                new Audit(0, DateTime.Now, MessageType.Error, "Invalid Timestamp for date " + DateTime.Now.ToString("yyyy-MM-dd HH-mm"))
                            );
                        }
                        else if (!int.TryParse(rowSplit[0], out int id))
                        {
                            errors.Add(
                                new Audit(0, DateTime.Now, MessageType.Error, "Invalid Id for date " + DateTime.Now.ToString("yyyy-MM-dd HH-mm"))
                            );
                        }
                        else
                        {
                            if (!double.TryParse(rowSplit[2], out double vrednost))
                            {
                                errors.Add(
                                new Audit(0, DateTime.Now, MessageType.Error, "Invalid Measured Value for date " + vreme.ToString("yyyy-MM-dd HH-mm"))
                            );
                            }
                            else
                            {
                                if (vrednost < 0.0)
                                {
                                    errors.Add(
                                        new Audit(0, DateTime.Now, MessageType.Warning, "Measured Value negative for date " + vreme.ToString("yyyy-MM-dd"))
                                    );

                                }

                                else
                                {
                                    values.Add(
                                        new Load(id, vreme, vrednost)
                                    );
                                }
                            }
                        }
                    }
                    line++;
                }
                stream.Dispose();
            }
            if (errors.Count == line - 1)
            {
                errors.Clear();
                errors.Add(
                        new Audit(0, DateTime.Now, MessageType.Error, "Invalid datastructure in CSV file " + DateTime.Now.ToString("yyyy-MM-dd"))
                );

                return false;
            }

            db.Write(values, errors, ConfigurationManager.AppSettings["DBLoads"], ConfigurationManager.AppSettings["DBAudits"]);

            if (errors.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
