using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace identity_connect.Exceptions
{
    public class StatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Response { get; set; }

        public override string Message => $"Статус ответа: {StatusCode}, ответ сервиса: {Response}";

        public StatusCodeException(HttpStatusCode code, string response)
        {
            StatusCode = code;
            Response = response;
        }
    }
}
