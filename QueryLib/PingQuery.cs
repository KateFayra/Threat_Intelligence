using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace QueryLib
{
    public class PingQuery : IQuery
    {
        private static string name = QueryParams.Ping;
        private readonly string ip;

        public PingQuery(string ip)
        {
            this.ip = ip;
        }

        public async Task<JObject> Query() 
        {
            try
            {
                return await Task.Run(() =>
                {
                    Ping pingSender = new Ping();
                    PingOptions options = new PingOptions();
                    options.DontFragment = true;

                    string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    int timeout = 120;
                    System.Net.IPAddress addr = System.Net.IPAddress.Parse(this.ip);
                    PingReply reply = pingSender.Send(addr, timeout, buffer, options);

                    if (reply.Status == IPStatus.Success)
                    {
                        return new JObject(new JProperty("Status", Enum.GetName(typeof(IPStatus), reply.Status)), new JProperty("RoundTripTime", reply.RoundtripTime), new JProperty("TimeToLive", reply.Options.Ttl), new JProperty("NumBytes", reply.Buffer.Length));
                    }
                    else
                    {
                        return new JObject(new JProperty("Status", Enum.GetName(typeof(IPStatus), reply.Status)), new JProperty("RoundTripTime", reply.RoundtripTime), new JProperty("TimeToLive", null), new JProperty("NumBytes", null));
                    }
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