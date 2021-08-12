using Nager.PublicSuffix;
using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Threading.Tasks;

namespace QueryLib
{
    public class RDAPQuery : IQuery
    {
        private static string name = QueryParams.RDAP;
        private readonly string url;
        private readonly string urlCom = "https://rdap.verisign.com/com/v1/domain/";
        private readonly string urlNet = "https://rdap.verisign.com/net/v1/domain/";

        public RDAPQuery(string domain)
        {
            if(domain.ToLower().EndsWith(".com"))
            {
                this.url = urlCom + domain;
            }
            else if(domain.ToLower().EndsWith(".net"))
            {
                this.url = urlNet + domain;
            }
            else
            {
                DomainParser domainParser = new DomainParser(new WebTldRuleProvider());
                DomainInfo domainInfo = domainParser.Parse(domain);
                this.url = "https://tld-rdap.verisign.com/" + domainInfo.TLD + "/v1/domain/" + domain;
            }
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