using System;
using System.Collections.Generic;
using System.Text;
using identity_connect.Models.Information;

namespace identity_connect.Models.Response
{
    public class OkResponseLogin : Response
    {
        public string AcceptKey { get; set; }

        public string RefreshKey { get; set; }

        public DateTime DeathTime { get; set; }

        public IList<string> Permissions { get; set; }

        public SimpleUserInfo Info { get; set; }
    }

    public class OkResponseToken : Response
    {
        public string Token { get; set; }

        public string Name { get; set; }

        public string ExternalId { get; set; }
    }
}
