using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praque_parking_oop_G
{
    public class Car : Vehicle 
    {
        public override string Type => "Car";
        public Car(string regNumber) : base(regNumber) { } 
    }
}
