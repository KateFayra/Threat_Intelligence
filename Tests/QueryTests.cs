using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using QueryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class QueryTests
    {
        string ip = "54.186.91.102";
        string domain = "njlochner.com";

        [TestMethod()]
        public async Task AbuseIPDBTest()
        {
            AbuseIPDBQuery query = new AbuseIPDBQuery(ip);
            JObject result = await query.Query();
            Assert.AreEqual(result["data"]["ipAddress"], ip);
        }

        [TestMethod()]
        public async Task ReverseDNSTest()
        {
            ReverseDNSQuery query = new ReverseDNSQuery(ip);
            JObject result = await query.Query();
            Assert.AreEqual(result["hostname"], "ec2-54-186-91-102.us-west-2.compute.amazonaws.com");
        }

        [TestMethod()]
        public async Task GeoIPTest()
        {
            GeoIPQuery query = new GeoIPQuery(ip);
            JObject result = await query.Query();
            Assert.AreEqual(result["status"], "success");
        }

        [TestMethod()]
        public async Task PingTest()
        {
            PingQuery query = new PingQuery("8.8.8.8");
            JObject result = await query.Query();
            Assert.AreEqual(result["Status"], "Success");
        }

        [TestMethod()]
        public async Task ShowdanTest()
        {
            ShodanQuery query = new ShodanQuery(ip);
            JObject result = await query.Query();
            Assert.AreEqual(result["ip_str"], ip);
        }

        [TestMethod()]
        public async Task DNSTest()
        {
            DNSQuery query = new DNSQuery(domain);
            JObject result = await query.Query();
            Assert.AreEqual(result["ips"]["0"], ip);
        }

        [TestMethod()]
        public async Task RDAPTest()
        {
            RDAPQuery query = new RDAPQuery(domain);
            JObject result = await query.Query();
            Assert.AreEqual(result["ldhName"], domain.ToUpper());
        }

        [TestMethod()]
        public async Task WhoisTest()
        {
            WhoisQuery query = new WhoisQuery(domain);
            JObject result = await query.Query();
            Assert.AreEqual(result["WhoisRecord"]["domainName"], domain);
        }
    }
}