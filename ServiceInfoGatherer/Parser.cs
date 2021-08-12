using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ServiceInfoGatherer
{
    public class Parser
    {
        private static void AddProp(string prop, string value, ref List<JProperty> props, ref int numQueries, NameValueCollection requestParams, bool defaultParam = true)
        {
            if ((defaultParam && requestParams.Count == 0) || (!String.IsNullOrWhiteSpace(requestParams[prop]) && requestParams[prop].ToLower() == "true"))
            {
                props.Add(new JProperty(prop, value));
                numQueries++;
            }
        }

        public static string[] BuildParamJson(string ip, string domain, int nonce, ref Tracker tracker, NameValueCollection requestParams, int numWorkers)
        {
            List<JProperty> props = new List<JProperty>();
            int numQueries = 0;

            if (!String.IsNullOrWhiteSpace(ip))
            {
                AddProp(QueryParams.GeoIP, ip, ref props, ref numQueries, requestParams);
                AddProp(QueryParams.ReverseDNS, ip, ref props, ref numQueries, requestParams);
                AddProp(QueryParams.Ping, ip, ref props, ref numQueries, requestParams);
                AddProp(QueryParams.AbuseIPDB, ip, ref props, ref numQueries, requestParams);
                AddProp(QueryParams.Shodan, ip, ref props, ref numQueries, requestParams, false);
            }
            else if (!String.IsNullOrWhiteSpace(domain))
            {
                AddProp(QueryParams.DNS, domain, ref props, ref numQueries, requestParams);
                AddProp(QueryParams.Whois, domain, ref props, ref numQueries, requestParams);
                AddProp(QueryParams.RDAP, domain, ref props, ref numQueries, requestParams);
            }

            if(props.Count == 0)
            {
                return null;
            }

            tracker.numQueries = numQueries;
            var split = LinqExtensions.Split(props, numWorkers);

            List<string> jsonList = new List<string>();
            foreach (var propEnum in split)
            {
                List<JProperty> propList = propEnum.ToList();
                propList.Add(new JProperty(QueryParams.Nonce, nonce));
                jsonList.Add(new JObject(propList.ToArray()).ToString());
            }

            return jsonList.ToArray();
        }
    }
}