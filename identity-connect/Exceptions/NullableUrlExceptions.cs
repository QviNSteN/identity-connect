using System;
using System.Collections.Generic;
using System.Text;

namespace identity_connect.Exceptions
{
    public class NullableUrlExceptions : Exception
    {
        public string Url { get; set; }

        public override string Message => "Адрес сервиса идентификации неверен!";

        public NullableUrlExceptions(string? url)
        {
            Url = url;
        }
    }
}
