using Newtonsoft.Json.Linq;
using QueryLib;
using ResourceLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string port = args[0];
            Console.WriteLine("Listening on port: " + port);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
            listener.Start();
            while (listener.IsListening)
            {
                var context = listener.GetContext();
                ProcessRequest(context);
            }
            listener.Close();
        }

        private static async void ProcessRequest(HttpListenerContext context)
        {
            int nonce = -1;
            try
            {
                Console.WriteLine("Processing Request");

                string body = new StreamReader(context.Request.InputStream).ReadToEnd();
                HTTPMethods.HTTPResponse(context, new JObject(new JProperty("Status", "200")).ToString(), 200);

                List<IQuery> queryObjects = QueryMethods.CreateQueryObjects(body, ref nonce);
                Dictionary<string, JObject> results = await QueryMethods.ExecuteQueries(queryObjects);

                List<JProperty> props = new();
                props.Add(new JProperty(QueryParams.Nonce, nonce));

                foreach (var result in results)
                {
                    props.Add(new JProperty(result.Key, result.Value));
                }

                string jsonResults = new JObject(props.ToArray()).ToString();

                Console.WriteLine("Sending JSON");
                await HTTPMethods.Post("http://127.0.0.1:8080/post", jsonResults);
                Console.WriteLine("Sent JSON");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception, sending HTTP 500");
                try
                {
                    await HTTPMethods.Post("http://127.0.0.1:8080/post", new JObject(new JProperty(QueryParams.Nonce, nonce), new JProperty("Status", "500"), new JProperty("Error", ex.Message)).ToString());
                }
                catch
                {

                }
            }
}
    }
}
