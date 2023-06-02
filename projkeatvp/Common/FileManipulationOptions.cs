using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class FileManipulationOptions : IDisposable
    {
        public FileManipulationOptions(MemoryStream ms, string fileName)
        {
            this.MS = ms;
            this.FileName = fileName;
        }
        public FileManipulationOptions()
        {
            this.MS = new MemoryStream();
        }

        [DataMember]
        public MemoryStream MS { get; set; }

        [DataMember]
        public string FileName { get; set; }
        public void Dispose()
        {
            if (MS == null)
                return;
            try
            {
                MS.Dispose();
                MS.Close();
                MS = null;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Unsuccesful disposing!");
            }
        }
    }
}
