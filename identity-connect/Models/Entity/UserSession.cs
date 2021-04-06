using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace identity_connect.Models.Entity
{
    public class UserSession : Session
    {
        public string AcceptKey { get; set; }

        public string RefreshKey { get; set; }

        public DateTime? DeathTime { get; set; }
    }
}
