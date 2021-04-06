using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using identity_connect.Expansions;
using identity_connect.Exceptions;
using identity_connect.Models.Response;
using identity_connect.Interfaces;

namespace identity_connect.Http
{
    public class Request : ISenderData
    {
        public Exception Exception { get; private set; }

        public object Result { get; private set; }

        public bool IsOk { get; private set; }

        public Request() { }

        public async Task Post(object data, string url, string token = null) => await Post<object>(data, url, token);

        public async Task<T> Post<T>(object data, string url, string token = null)
        {
            if (String.IsNullOrEmpty(url))
                return Throw<T>(new NullableUrlExceptions(url));

            try
            {
                var body = data.ToJson();
                HttpResponseMessage responseMessage = null;
                using (HttpClient httpClient = new HttpClient())
                {
                    StringContent httpConent = new StringContent(body, Encoding.UTF8, "application/json");

                    if (!String.IsNullOrEmpty(token))
                        httpClient.DefaultRequestHeaders.Add("Token", token);
                    responseMessage = await httpClient.PostAsync(url, httpConent);

                    var json = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.StatusCode != HttpStatusCode.OK && responseMessage.StatusCode != HttpStatusCode.Created)
                        throw new StatusCodeException(responseMessage.StatusCode, json);

                    if (typeof(T) == typeof(object))
                        return default(T);

                    return json.ToObject<T>();
                }
            }
            catch (Exception e)
            {
                return Throw<T>(e);
            }
        }

        public async Task<T> Get<T>(string url, string token = null)
        {
            if (String.IsNullOrEmpty(url))
                return Throw<T>(new NullableUrlExceptions(url));

            using (HttpClient httpClient = new HttpClient())
            {
                if (!String.IsNullOrEmpty(token))
                    httpClient.DefaultRequestHeaders.Add("Token", token);
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                return content.ToObject<T>();
            }
        }

        private T Throw<T>(Exception e)
        {
            Exception = e;
            IsOk = false;

            return default(T);
        }
    }
}
