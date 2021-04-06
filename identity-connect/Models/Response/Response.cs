using System;
using System.Collections.Generic;
using System.Text;

namespace identity_connect.Models.Response
{
    public class Response
    {
        public bool IsAuthorized { get; set; }

        public string ErrorMessage { get; set; }
    }
}
