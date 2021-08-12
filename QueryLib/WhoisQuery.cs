using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Threading.Tasks;

namespace QueryLib
{
    public class WhoisQuery : IQuery
    {
        private static string name = QueryParams.Whois;
        private static readonly string apiKey = "YOUR_API_KEY_HERE"; // TODO: Read from file
        private readonly string url = "https://www.whoisxmlapi.com/whoisserver/WhoisService?outputFormat=JSON&apiKey=" + apiKey;

        public WhoisQuery(string domain)
        {
            this.url += "&domainName=" + domain;
        }

        public async Task<JObject> Query()
        {
            return await HTTPMethods.JSONQuery(this.url);
        }

        public string GetName()
        {
            return name;
        }
    }

}