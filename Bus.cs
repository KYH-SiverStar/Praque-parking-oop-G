using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praque_parking_oop_G
{
    public class Bus : Vehicle
    { 
        public override string Type => "Bus";
        public Bus(string regNumber) : base(regNumber) { } 
    }
}
