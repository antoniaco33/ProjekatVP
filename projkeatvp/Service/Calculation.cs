using Common.Models;
using DataBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Calculation
    {
        public delegate double CalculationDelegateHandler(object sender, CalculationEventArgs x);
        public event CalculationDelegateHandler MinEventHandler;
        public event CalculationDelegateHandler MaxEventHandler;
        public event CalculationDelegateHandler DevEventHandler;
        public Calculation()
        {
            MinEventHandler += Calculator_MinEventHandler;
            MaxEventHandler += Calculator_MaxEventHandler;
            DevEventHandler += Calculator_DevEventHandler;
        }

        public List<double> InvokeEvent(string command, List<double> data)
        {
            List<double> ret = new List<double>();
            if (command.ToLower().Contains("min"))
                ret.Add(MinEventHandler.Invoke(this, new CalculationEventArgs { Data = data }));
            if (command.ToLower().Contains("max"))
                ret.Add(MaxEventHandler.Invoke(this, new CalculationEventArgs { Data = data }));
            if (command.ToLower().Contains("stand"))
                ret.Add(DevEventHandler.Invoke(this, new CalculationEventArgs { Data = data }));

            return ret;

        }
        

        private double Calculator_DevEventHandler(object sender, CalculationEventArgs x)
        {
            double sum = 0;
            foreach (var value in x.Data)
            {
                sum += value;
            }
            double mean = sum / x.Data.Count;

            double sumSquaredDifferences = 0;
            foreach (var value in x.Data)
            {
                double difference = value - mean;
                sumSquaredDifferences += difference * difference;
            }

            double variance = sumSquaredDifferences / x.Data.Count;
            double standardDeviation = Math.Sqrt(variance);

            return standardDeviation;
        }

        private double Calculator_MaxEventHandler(object sender, CalculationEventArgs x)
        {
            double maxRet = x.Data[0];
            foreach (var value in x.Data)
            {
                if (value > maxRet)
                {
                    maxRet = value;
                }
            }
            return maxRet;
        }

        private double Calculator_MinEventHandler(object sender, CalculationEventArgs x)
        {
            double minRet = x.Data[0];
            foreach (var value in x.Data)
            {
                if (value < minRet)
                {
                    minRet = value;
                }
            }
            return minRet;
        }

        public List<double> ProcessData(string command, List<double> data)
        {
            return InvokeEvent(command, data);
        }
    }
}
