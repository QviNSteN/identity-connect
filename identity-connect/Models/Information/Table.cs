using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace identity_connect.Models.Information
{
    public class Table
    {
        public string Name { get; set; }

        public Collumn[] Collumns { get; set; }
    }

    public class Collumn
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}
