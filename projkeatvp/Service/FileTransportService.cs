using Common;
using Common.Models;
using DataBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class FileTransportService : IFileTransport
    {
        private readonly string csvFilePath;
        private readonly Database database;
        public FileTransportService(string csvFilePath)
        {
            this.csvFilePath = csvFilePath;
            this.database = new Database();
        }
        public void SendDataToServer()
        {
            // Perform sending of the CSV file to the server
            // Code for sending the file to the server goes here

            // Once the file is successfully sent, parse the data and store it in the database
            ParseAndStoreData();
        }

        private void ParseAndStoreData()
        {
            try
            {
                // Code for parsing the CSV file and storing the data in the database goes here
                // Example: Read the CSV file, parse its contents, create Load objects, and store them in the database

                using (var reader = new StreamReader(csvFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        if (values.Length == 3 && int.TryParse(values[0], out int id) &&
                            DateTime.TryParse(values[1], out DateTime timestamp) &&
                            float.TryParse(values[2], out float measuredValue))
                        {
                            var load = new Load
                            {
                                Id = id,
                                Timestamp = timestamp,
                                MeasuredValue = measuredValue
                            };

                            Database.Loads.Add(load);
                        }
                    }
                }

                database.SaveToXml(ConfigurationManager.AppSettings.Get("PathCSV"));
                Console.WriteLine("Data stored in the database successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while parsing and storing the data: " + ex.Message);
            }
        }

    }
}
