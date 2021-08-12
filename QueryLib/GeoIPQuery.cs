using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Threading.Tasks;

namespace QueryLib
{
    public class GeoIPQuery : IQuery
    {
        private static string name = QueryParams.GeoIP;
        private string url = "http://ip-api.com/json/";

        public GeoIPQuery(string ip)
        {
            this.url += ip;
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