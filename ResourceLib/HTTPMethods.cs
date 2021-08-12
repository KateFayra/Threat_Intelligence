using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ResourceLib
{
    public class HTTPMethods
    {
        public static async Task<JObject> JSONQuery(string url, string apiKey = null)
        {
            JObject details;

            try
            {
                HttpClient client = new HttpClient(new HttpClientHandler { Proxy = null, UseProxy = false });

                client.DefaultRequestHeaders.Accept.Clear();

                if (!String.IsNullOrWhiteSpace(apiKey))
                {
                    client.DefaultRequestHeaders.Add("Key", apiKey);
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Task<string> stringTask = client.GetStringAsync(url);

                string msg = await stringTask;
                details = JObject.Parse(msg);
            }
            catch (Exception ex)
            {
                details = new JObject(new JProperty("Error", ex.Message));
            }

            return details;
        }

        public static async Task Post(string url, string json)
        {
            HttpClient client = new HttpClient(new HttpClientHandler { Proxy = null, UseProxy = false });
            var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public static void HTTPResponse(HttpListenerContext context, string body, int statuscode)
        {
            byte[] b = Encoding.UTF8.GetBytes(body);
            context.Response.StatusCode = statuscode;
            context.Response.KeepAlive = false;
            context.Response.ContentLength64 = b.Length;
            context.Response.ContentType = "application/json";

            var output = context.Response.OutputStream;
            output.Write(b, 0, b.Length);
            context.Response.Close();
        }

    }
}
