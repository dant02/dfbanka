using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dfbanka.gui.api
{
    internal class WordPress
    {
        private static void SetAuthenticationHeader(WebRequest request, string username, string password)
        {
            request.PreAuthenticate = true;
            var encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            request.Headers.Add("Authorization", "Basic " + encoded);
        }

        public static async Task Put(string username, string password, string url, string payload)
        {
            var request = WebRequest.Create(url);
            SetAuthenticationHeader(request, username, password);
            request.Method = "PUT";
            request.ContentType = "application/json";

            if (payload != null)
            {
                request.ContentLength = payload.Length;
                using (Stream stream = request.GetRequestStream())
                using (var writer = new StreamWriter(stream))
                    await writer.WriteAsync(payload);
            }

            string result = null;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                result = response.StatusCode.ToString();
        }

        public static async Task<string> Get(string username, string password, string url)
        {
            WebRequest request = WebRequest.Create(url);
            SetAuthenticationHeader(request, username, password);

            string responseStr = string.Empty;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                responseStr = reader.ReadToEnd();

            return responseStr;
        }
    }
}