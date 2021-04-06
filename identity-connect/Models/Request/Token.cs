using System;
using System.Collections.Generic;
using System.Text;

namespace identity_connect.Models.Request
{
    public class TokenAuth : Auth
    {
        public string Token { get; set; }

        public string Url { get; set; }
    }
}
