using System;
using System.Collections.Generic;
using System.Text;
using identity_connect.Data.Enums;

namespace identity_connect.Models.Information
{
    public class IntegrationElement
    {
        public TypeIntegrationEnum Type { get; set; }

        public string Value { get; set; }
    }
}
