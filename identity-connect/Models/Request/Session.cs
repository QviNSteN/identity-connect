using System;
using System.Collections.Generic;
using System.Text;

namespace identity_connect.Models.Request
{
    public class SessionAuth : Auth
    {
        public string AcceptKey { get; set; }

        public string RefreshKey { get; set; }
    }
}
