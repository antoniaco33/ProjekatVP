using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceModel;

namespace UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("=============================================================");
                Console.WriteLine("Please enter one of the commands below");
                Console.WriteLine("For sending CSV file data to the Database, enter Send");
                Console.WriteLine("For reading data from the Database, enter Get followed by any combination of min/max/stand with a space between");
                Console.WriteLine("To exit the application enter Exit");

                string clientCommand = Console.ReadLine();
                string[] splitCommand = clientCommand.Split(' ');
                if (clientCommand.ToLower() == "send")
                {
                    Console.WriteLine("Sending data to the server...");
                    try
                    {
                        List<Audit> errors = new List<Audit>();

                        errors = SendCSV();
                        if (errors.Count == 0)
                        {
                            Console.WriteLine("Data successfully entered into database!");
                        }
                        else
                        {
                            Console.WriteLine("Part of data successfully entered into database!");
                            Console.WriteLine("List of errors encountered during data transfer:");
                        }
                        foreach (Audit audit in errors)
                        {
                            Console.WriteLine(audit.Message);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (splitCommand.Length <= 4 && splitCommand[0].ToLower() == "get")
                {
                    try
                    {
                        GetValues(clientCommand);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                else if (clientCommand.ToLower() == "exit")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid command. Please try again");
                }


            }
        }

        private static List<Audit> SendCSV()
        {
            bool success = false;
            string csvPath = ConfigurationManager.AppSettings["CsvDirektorijum"];

            MemoryStream memoryStream = new MemoryStream();
            List<Audit> errors = new List<Audit>();

            ChannelFactory<IFileTransport> xmlCsvChannelFactory = new ChannelFactory<IFileTransport>("Server");
            IFileTransport csvProxy = xmlCsvChannelFactory.CreateChannel();


            using (FileStream csvFileStream = new FileStream(csvPath, FileMode.Open, FileAccess.Read))
            {
                csvFileStream.CopyTo(memoryStream);
                csvFileStream.Dispose();
            }

            memoryStream.Position = 0;

            using (FileManipulationOptions options = new FileManipulationOptions(memoryStream, "data"))
            {
                success = csvProxy.ParseFile(options, out errors);
                options.Dispose();
                memoryStream.Dispose();
            }

            if (success && File.Exists(csvPath))
            {
                File.Delete(csvPath);
                Console.WriteLine("File " + csvPath + " successfully deleted!");
            }
            return errors;
        }

        private static void GetValues(string commands)
        {
            ChannelFactory<IFileTransport> xmlCsvChannelFactory = new ChannelFactory<IFileTransport>("Server");
            IFileTransport csvProxy = xmlCsvChannelFactory.CreateChannel();

            FileManipulationOptions options = csvProxy.GetCalculations(commands);
            string txtFilePath = ConfigurationManager.AppSettings["TxtDirektorijum"];
            txtFilePath += options.FileName;

            using (var fileStream = new FileStream(txtFilePath, FileMode.Create, FileAccess.Write))
            {
                options.MS.WriteTo(fileStream);
                fileStream.Dispose();
            }

            Console.WriteLine($"Txt file {options.FileName} successfully made. File location: {txtFilePath}");

            options.Dispose();
        }

    }
}
