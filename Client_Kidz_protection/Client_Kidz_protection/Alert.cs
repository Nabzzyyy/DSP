using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Kidz_protection
{
    internal struct Alert
    {
        public string Keyword { set; get; }
        public string Screenshot { set; get; } // base 64 encoded
        public int AlertTypeID { set; get; }
        public string ClientHash { set; get; }

        public string Location { set; get; } 
    }
}
