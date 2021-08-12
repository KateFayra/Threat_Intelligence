using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Net;
using System.Threading.Tasks;

namespace QueryLib
{
    public class ReverseDNSQuery : IQuery
    {
        private static string name = QueryParams.ReverseDNS;
        private readonly string ip;

        public ReverseDNSQuery(string ip)
        {
            this.ip = ip;
        }

        public async Task<JObject> Query()
        {
            try
            {
                return await Task.Run(() =>
                {
                    System.Net.IPAddress addr = System.Net.IPAddress.Parse(this.ip);
                    IPHostEntry entry = Dns.GetHostEntry(addr);

                    return new JObject(new JProperty("hostname", entry.HostName));
                });
            }
            catch (Exception ex)
            {
                return new JObject(new JProperty("Error", ex.Message));
            }
        }
        public string GetName()
        {
            return name;
        }
    }

}