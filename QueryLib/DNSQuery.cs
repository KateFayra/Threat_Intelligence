using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace QueryLib
{
    public class DNSQuery : IQuery
    {
        private static string name = QueryParams.DNS;
        private readonly string domain;

        public DNSQuery(string domain)
        {
            this.domain = domain;
        }

        public async Task<JObject> Query()
        {
            try
            {
                return await Task.Run(() =>
                {
                    List<JProperty> props = new List<JProperty>();

                    int i = 0;
                    foreach (System.Net.IPAddress ip in Dns.GetHostAddresses(this.domain))
                    {
                        props.Add(new JProperty(i.ToString(), ip.ToString()));
                        i++;
                    }

                    return new JObject(new JProperty("ips", new JObject(props.ToArray())));
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