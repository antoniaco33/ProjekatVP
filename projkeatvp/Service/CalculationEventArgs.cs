using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CalculationEventArgs : EventArgs
    {
        public List<double> Data { get; set; }
    }
}
