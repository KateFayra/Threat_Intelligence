using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Threading.Tasks;

namespace QueryLib
{
    public class AbuseIPDBQuery : IQuery
    {
        // API doc: https://docs.abuseipdb.com/?csharp#check-parameters

        private static string name = QueryParams.AbuseIPDB;
        private readonly string url = "https://api.abuseipdb.com/api/v2/check";
        private readonly string apiKey = "YOUR_API_KEY_HERE"; // TODO read from file.

        public AbuseIPDBQuery(string ip)
        {
            this.url += "?ipAddress=" + ip;
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