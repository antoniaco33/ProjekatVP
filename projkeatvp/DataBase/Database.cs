using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DataBase
{
    public class Database
    {
        private int ID;

        public List<Load> Read(string path)
        {
            List<Load> loads = new List<Load>();

            using (FileStream fs = OpenFile(path))
            {
                XmlDocument db = new XmlDocument();
                db.Load(fs);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                XmlNodeList rows = db.SelectNodes($"//row[TIME_STAMP = '{date}']");

                ID = 1;
                foreach (XmlNode row in rows)
                {
                    Load load = new Load(ID++, DateTime.Parse(row.SelectSingleNode("TIME_STAMP").InnerText), double.Parse(row.SelectSingleNode("MEASURED_VALUE").InnerText));

                    loads.Add(load);
                }

                fs.Dispose();
            }

            return loads;
        }

        public void Write(List<Load> loads, List<Audit> audits, string path)
        {
            WriteAudit(audits, path);

            WriteLoad(loads, path);
        }

        private void WriteAudit(List<Audit> audits, string path)
        {
            using (FileStream fs = OpenFile(path))
            {
                XmlDocument db = new XmlDocument();
                db.Load(fs);

                XmlNodeList rows = db.SelectNodes("//STAVKA");
                int maxID = rows.Count;

                foreach (Audit a in audits)
                {
                    a.Id = ++maxID;

                    XmlElement newRow = db.CreateElement("STAVKA");

                    XmlElement idElement = db.CreateElement("ID");
                    idElement.InnerText = a.Id.ToString();

                    XmlElement timeStampElement = db.CreateElement("TIME_STAMP");
                    timeStampElement.InnerText = a.Timestamp.ToString("yyyy-MM-dd");

                    XmlElement messageTypeElement = db.CreateElement("MESSAGE_TYPE");
                    messageTypeElement.InnerText = a.Type.ToString();

                    XmlElement messageElement = db.CreateElement("MESSAGE");
                    messageElement.InnerText = a.Message;

                    newRow.AppendChild(idElement);
                    newRow.AppendChild(timeStampElement);
                    newRow.AppendChild(messageTypeElement);
                    newRow.AppendChild(messageElement);

                    XmlElement rootElement = db.DocumentElement;
                    rootElement.AppendChild(newRow);
                    db.Save(path);
                }

                if (audits.Count == 0)
                {
                    XmlElement newRow = db.CreateElement("STAVKA");

                    XmlElement idElement = db.CreateElement("ID");
                    idElement.InnerText = (++maxID).ToString();

                    XmlElement timeStampElement = db.CreateElement("TIME_STAMP");
                    timeStampElement.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    XmlElement messageTypeElement = db.CreateElement("MESSAGE_TYPE");
                    messageTypeElement.InnerText = "Info";

                    XmlElement messageElement = db.CreateElement("MESSAGE");
                    messageElement.InnerText = "Data successfully entered into database";

                    newRow.AppendChild(idElement);
                    newRow.AppendChild(timeStampElement);
                    newRow.AppendChild(messageTypeElement);
                    newRow.AppendChild(messageElement);

                    XmlElement rootElement = db.DocumentElement;
                    rootElement.AppendChild(newRow);
                    db.Save(path);
                }

                fs.Dispose();
            }
        }

        private void WriteLoad(List<Load> podaci, string path)
        {
            using (FileStream fs = OpenFile(path))
            {
                XmlDocument db = new XmlDocument();
                db.Load(fs);

                fs.Position = 0;

                XmlNodeList rows = db.SelectNodes("//row");
                int maxID = rows.Count;

                foreach (Load l in podaci)
                {
                    XmlNode element = null;

                    try
                    {
                        element = db.SelectSingleNode($"//row[TIME_STAMP = '{l.Timestamp.ToString("yyyy-MM-dd HH:mm")}']");
                    }
                    catch { }


                    if (element != null)
                    {
                        element.SelectSingleNode("MEASURED_VALUE").InnerText = l.MeasuredValue.ToString();
                        db.Save(path);
                    }
                    else
                    {
                        XmlElement newRow = db.CreateElement("row");

                        XmlElement idElement = db.CreateElement("ID");
                        idElement.InnerText = (++maxID).ToString();

                        XmlElement timeStampElement = db.CreateElement("TIME_STAMP");
                        timeStampElement.InnerText = l.Timestamp.ToString("yyyy-MM-dd");

                        XmlElement measuredValueElement = db.CreateElement("MEASURED_VALUE");
                        measuredValueElement.InnerText = l.MeasuredValue.ToString();

                        newRow.AppendChild(idElement);
                        newRow.AppendChild(timeStampElement);
                        newRow.AppendChild(measuredValueElement);

                        XmlElement rootElement = db.DocumentElement;
                        rootElement.AppendChild(newRow);
                        db.Save(path);
                    }
                }

                fs.Dispose();
            }
        }


        public FileStream OpenFile(string path)
        {
            if (!File.Exists(path))
            {
                string start = "";
                if (path.Contains("audit"))
                    start = "STAVKE";
                else
                    start = "rows";

                XDocument xml = new XDocument(new XElement(start));
                xml.Save(path);
            }

            return new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
        }
    }
}
