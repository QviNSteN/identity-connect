using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace identity_connect.Models.Entity
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public IList<string> Permissions { get; set; } = new List<string>();
    }
}