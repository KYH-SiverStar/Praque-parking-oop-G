using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praque_parking_oop_G
{
    public abstract class Vehicle
    {
        public string RegNumber { get; set; }
        public DateTime ArrivalTime { get; set; }
        public abstract string Type { get; }
        public Vehicle(string regNumber)
        {
            RegNumber = regNumber.ToLower();
            ArrivalTime = DateTime.Now;
        }
    }
}

  