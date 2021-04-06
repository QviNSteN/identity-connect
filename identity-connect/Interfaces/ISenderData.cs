using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace identity_connect.Interfaces
{
    public interface ISenderData
    {
        Task Post(object data, string url, string token = null);

        Task<T> Post<T>(object data, string url, string token = null);

        Task<T> Get<T>(string url, string token = null);
    }
}
