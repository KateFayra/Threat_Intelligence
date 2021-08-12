using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Threading.Tasks;

namespace QueryLib
{
    public class ShodanQuery : IQuery
    {
        private static string name = QueryParams.Shodan;
        private static readonly string apiKey = "YOUR_API_KEY_HERE"; // TODO read from file.
        private readonly string url = "https://api.shodan.io/shodan/host/";

        public ShodanQuery(string ip)
        {
            this.url += ip + "?key=" + apiKey;
        }

        public async Task<JObject> Query()
        {
            return await HTTPMethods.JSONQuery(this.url, apiKey);
        }
        public string GetName()
        {
            return name;
        }
    }

}