using Newtonsoft.Json.Linq;
using QueryLib;
using ResourceLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worker
{
    public class QueryMethods
    {
        public static List<IQuery> CreateQueryObjects(string json, ref int nonce)
        {
            List<IQuery> queryObjects = new List<IQuery>();
            JObject parsedJson = JObject.Parse(json);
            foreach (var pair in parsedJson)
            {
                string key = pair.Key;
                string data = pair.Value.ToString();

                switch (key)
                {
                    case QueryParams.Nonce:
                        nonce = Int32.Parse(data);
                        break;
                    case QueryParams.GeoIP:
                        queryObjects.Add(new GeoIPQuery(data));
                        break;
                    case QueryParams.ReverseDNS:
                        queryObjects.Add(new ReverseDNSQuery(data));
                        break;
                    case QueryParams.Ping:
                        queryObjects.Add(new PingQuery(data));
                        break;
                    case QueryParams.AbuseIPDB:
                        queryObjects.Add(new AbuseIPDBQuery(data));
                        break;
                    case QueryParams.Shodan:
                        queryObjects.Add(new ShodanQuery(data));
                        break;
                    case QueryParams.DNS:
                        queryObjects.Add(new DNSQuery(data));
                        break;
                    case QueryParams.Whois:
                        queryObjects.Add(new WhoisQuery(data));
                        break;
                    case QueryParams.RDAP:
                        queryObjects.Add(new RDAPQuery(data));
                        break;
                }
            }

            return queryObjects;
        }

        public static async Task<Dictionary<string, JObject>> ExecuteQueries(List<IQuery> queryObjects)
        {
            List<Task<JObject>> tasks = new List<Task<JObject>>();

            foreach (IQuery queryObject in queryObjects)
            {
                tasks.Add(queryObject.Query());
            }

            JObject[] result = await Task.WhenAll(tasks.ToArray());

            Dictionary<string, JObject> resultsDict = new Dictionary<string, JObject>();
            for (int i = 0; i < queryObjects.Count; i++)
            {
                resultsDict[queryObjects[i].GetName()] = result[i];
            }

            return resultsDict;
        }
    }
}
