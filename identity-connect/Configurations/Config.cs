using System;
using System.Collections.Generic;
using System.Text;

namespace identity_connect.Configurations
{
    public class Config
    {
        public string AuthUrl { get; set; }

        public string GetUserInfoUrl { get; set; }

        public string GetUserIntegrationTypesUrl { get; set; }

        public string Token { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}
