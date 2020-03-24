using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDemo
{
    public class ItemCount
    {
        public int Sub = 0;
        public int Whale = 0;
        public int Destroyer = 0;
        public int Cargo = 0;
        public int Total { get { return Sub + Whale + Destroyer + Cargo; } }
    }
}
