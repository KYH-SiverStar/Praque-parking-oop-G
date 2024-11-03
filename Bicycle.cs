using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praque_parking_oop_G
{
    public class Bicycle : Vehicle 
    { 
        public override string Type => "Bicycle";
        public Bicycle(string regNumber) : base(regNumber) { }
    }
}
