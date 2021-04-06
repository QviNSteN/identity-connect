using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace identity_connect.Expansions
{
    public static class StringExpansions
    {
        public static bool CanParseToInt(this string number)
        {
            try
            {
                int.Parse(number);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool CanParseToFloat(this string number)
        {
            try
            {
                float.Parse(number);
            }
            catch
            {
                return false;
            }

            return true;
        }

        //Смысла проверять как-то лучше никакого. 
        public static bool IsEmail(this string email) => email.CorrectRegex(@"^(\S+)@([a-z0-9-]+)(\.)([a-z]{2,4})(\.?)([a-z]{0,4})+$");

        public static bool IsPhone(this string phone) => phone.CorrectRegex(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");
        public static bool CorrectRegex(this string text, string regex) => Regex.IsMatch(text, regex);

        public static bool AllElementsOfNumber(this string[] elements) =>
            elements.All(x => x.CanParseToFloat());

        public static string[] ToArray(this string elements) => elements.Split(',');

        public static T ToObject<T>(this string json) => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        public async static Task<T> ToObject<T>(this Task<string> json) => JsonSerializer.Deserialize<T>(await json);

        public static string FixUrl(this string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                var port = url.Split(':', '/')[1];
                if (!String.IsNullOrEmpty(port))
                {
                    if (port == "8443" || port == "443")
                        url = "https://" + url;
                    else
                        url = "http://" + url;
                }
            }
            return url.EndsWith('/') ? url : url + '/';
        }

        public static string GenQuerty(this string url, params (string item, string value)[] p)
        {
            if (p.Length > 0)
            {
                if (url.Last() == '/')
                    url = url.Remove(url.Length - 1);
                return String.Format($"{url}?{String.Join('&', p.Select(x => String.Format($"{x.item}={x.value}")))}");
            }
            return url.FixUrl();
        }
    }
}
