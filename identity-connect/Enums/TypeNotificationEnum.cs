using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace identity_connect.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TypeIntegrationEnum
    {
        Email,
        Telegram,
        Amo
    }
}
