using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praque_parking_oop_G
{
    public class MC : Vehicle
    {
        public override string Type => "MC";
        public MC(string regNumber) : base(regNumber) { }
    }
}
