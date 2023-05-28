using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataBase
{
    public class Database
    {
        public static List<Load> Loads { get; set; }
        public static List<Audit> Audits { get; set; }

        public void SaveToXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Database));
            using (var writer = XmlWriter.Create(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static Database LoadFromXml(string fileName)
        {
            var serializer = new XmlSerializer(typeof(Database));
            using (var reader = XmlReader.Create(fileName))
            {
                return (Database)serializer.Deserialize(reader);
            }
        }
    }
}
