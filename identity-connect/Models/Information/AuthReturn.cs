using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using identity_connect.Models.Information;

namespace identity_connect.Models.Auth
{
    public class AuthReturn
    {
        public bool IsAuthorized { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class UserAuthReturn : AuthReturn
    {
        public string AcceptKey { get; set; }

        public string RefreshKey { get; set; }

        public SimpleUserInfo Info { get; set; }

        public IList<string> Permissions { get; set; }
    }

    public class ServiceAuthReturn : AuthReturn
    {
        public string Name { get; set; }

        public string ExternalId { get; set; }
    }
}
