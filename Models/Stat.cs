using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Models
{
    public class Stat
    {
        public int Customers { get; set; }

        public int Versions { get; set; }
        public int Admins { get; set; }
    }
}
