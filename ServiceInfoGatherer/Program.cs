using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace ServiceInfoGatherer
{
    public class Program
    {
        static int nonce = 0;
        static Dictionary<int, Tracker> trackers = new Dictionary<int, Tracker>();

        public static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8080/");
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
            try
            {
                string body = new StreamReader(context.Request.InputStream).ReadToEnd();
                Console.WriteLine("Handling request for: " + context.Request.RawUrl);

                if(context.Request.RawUrl.StartsWith("/ip/") || context.Request.RawUrl.StartsWith("/domain/"))
                {
                    int start = context.Request.RawUrl.LastIndexOf('/') + 1;
                    int end = context.Request.RawUrl.IndexOf('?');
                    string ip = null;
                    string domain = null;
                    string parsed;
                    if (end > 0)
                    {
                        parsed = context.Request.RawUrl.Substring(start, end - start);
                    }
                    else
                    {
                        parsed = context.Request.RawUrl.Substring(start);
                    }

                    if (context.Request.RawUrl.StartsWith("/ip/"))
                    {
                        ip = parsed;
                    }
                    else if (context.Request.RawUrl.StartsWith("/domain/"))
                    {
                        domain = parsed;
                    }

                    NameValueCollection requestParams = HttpUtility.ParseQueryString(new Uri(context.Request.Url.ToString()).Query);

                    await SendTasks(context, requestParams, ip, domain);
                
                }
                else if (context.Request.RawUrl == "/post") // TODO: Use authentication to prevent requests to here breaking things.
                {
                    JObject parsedJson = JObject.Parse(body);
                    int resultNonce = Int32.Parse(parsedJson[QueryParams.Nonce].ToString());
                    if (resultNonce > -1)
                    {
                        parsedJson.Remove(QueryParams.Nonce);
                        Tracker tracker = trackers[resultNonce];

                        var errorObj = parsedJson["Error"];
                        if(errorObj != null)
                        {
                            string error = errorObj.ToString();
                            if (!String.IsNullOrWhiteSpace(error))
                            {
                                HTTPMethods.HTTPResponse(tracker.context, new JObject(new JProperty("Status", "500"), new JProperty("Error", error)).ToString(), 500);
                            }
                        }

                        tracker.finishedQueries += parsedJson.Count;
                        tracker.responses.Add(parsedJson);

                        if (tracker.finishedQueries == tracker.numQueries)
                        {
                            JObject finalResponse = new JObject();
                            foreach (JObject response in tracker.responses)
                            {
                                finalResponse.Merge(response);
                            }

                            //Return to client
                            HTTPMethods.HTTPResponse(tracker.context, finalResponse.ToString(), 200);
                        }
                    }

                    HTTPMethods.HTTPResponse(context, new JObject(new JProperty("Status", "200")).ToString(), 200);
                }
                else
                {
                    HTTPMethods.HTTPResponse(context, new JObject(new JProperty("Status", "404")).ToString(), 404);
                }
            }
            catch (Exception ex)
            {
                HTTPMethods.HTTPResponse(context, new JObject(new JProperty("Status", "500"), new JProperty("Error", ex.Message)).ToString(), 500);
            }
        }

        private static async Task SendTasks(HttpListenerContext context, NameValueCollection requestParams, string ip, string domain)
        {
            Tracker tracker = new Tracker(context);
            string[] json = Parser.BuildParamJson(ip, domain, nonce, ref tracker, requestParams, Workers.numWorkers);

            if(json == null)
            {
                HTTPMethods.HTTPResponse(context, new JObject(new JProperty("Status", "400"), new JProperty("Error", "Bad Request")).ToString(), 400);
                return;
            }

            trackers[nonce] = tracker;
            nonce++;
            Console.WriteLine("Sending JSON to workers.");

            List<Task> tasks = new List<Task>();

            for(int i = 0; i < Workers.numWorkers; i++)
            {
                if (i < json.Length)
                {
                    tasks.Add(HTTPMethods.Post(Workers.workerURLs[i], json[i]));
                }
            }

            await Task.WhenAll(tasks.ToArray());

            Console.WriteLine("Sent JSON to workers.");
        }
    }
}