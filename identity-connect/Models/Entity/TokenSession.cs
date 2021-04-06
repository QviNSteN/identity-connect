using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace identity_connect.Models.Entity
{
    public class TokenSession : Session
    {
        public string Token { get; set; }

        public string Name { get; set; }

        public string ExternalId { get; set; }
    }
}
