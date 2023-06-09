﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Load
    {
        private int id;
        private DateTime timestamp;
        private double measuredValue;

        public int Id { get => id; set => id = value; }
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        public double MeasuredValue { get => measuredValue; set => measuredValue = value; }

        public Load(int id, DateTime timestamp, double measuredValue)
        {
            this.id = id;
            this.timestamp = timestamp;
            this.measuredValue = measuredValue;
        }

        public Load()
        {
        }
    }
}
