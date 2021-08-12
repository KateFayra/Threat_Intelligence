using Microsoft.VisualStudio.TestTools.UnitTesting;
using Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using ServiceInfoGatherer;
using System.Net;
using QueryLib;
using ResourceLib;

namespace Tests
{
    [TestClass()]
    public class ParseTests
    {
        string ip = "54.186.91.102";
        string domain = "njlochner.com";

        [TestMethod()]
        public void BuildParamJsonIPTest()
        {
            Tracker tracker = new Tracker(null);
            int nonce = 0;
            NameValueCollection requestParams = HttpUtility.ParseQueryString(new Uri("http://127.0.0.1:8080/ip/" + ip + "?Ping=true&GeoIP=true").Query);
            string[] json = Parser.BuildParamJson(ip, null, nonce, ref tracker, requestParams, Workers.numWorkers);

            Assert.AreEqual(json.Length, 2);
            Assert.AreEqual(json[0], "{\r\n  \"GeoIP\": \"" + ip + "\",\r\n  \"Nonce\": 0\r\n}");
            Assert.AreEqual(json[1], "{\r\n  \"Ping\": \"" + ip + "\",\r\n  \"Nonce\": 0\r\n}");
        }


        [TestMethod()]
        public void BuildParamJsonDomainTest()
        {
            Tracker tracker = new Tracker(null);
            int nonce = 0;
            NameValueCollection requestParams = HttpUtility.ParseQueryString(new Uri("http://127.0.0.1:8080/domain/" + domain + "?DNS=true&RDAP=true").Query);
            string[] json = Parser.BuildParamJson(null, domain, nonce, ref tracker, requestParams, Workers.numWorkers);

            Assert.AreEqual(json.Length, 2);
            Assert.AreEqual(json[0], "{\r\n  \"DNS\": \"" + domain + "\",\r\n  \"Nonce\": 0\r\n}");
            Assert.AreEqual(json[1], "{\r\n  \"RDAP\": \"" + domain + "\",\r\n  \"Nonce\": 0\r\n}");
        }


        [TestMethod()]
        public void CreateQueryObjectsTest()
        {
            Tracker tracker = new Tracker(null);
            int nonce = 0;
            NameValueCollection requestParams = HttpUtility.ParseQueryString(new Uri("http://127.0.0.1:8080/ip/" + ip + "?Ping=true&GeoIP=true").Query);
            string[] json = Parser.BuildParamJson(ip, null, nonce, ref tracker, requestParams, Workers.numWorkers);



            nonce = -1;
            List<IQuery> list = QueryMethods.CreateQueryObjects(json[0], ref nonce);
            Assert.AreEqual(nonce, 0);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].GetName(), QueryParams.GeoIP);

            nonce = -1;
            list = QueryMethods.CreateQueryObjects(json[1], ref nonce);
            Assert.AreEqual(nonce, 0);
            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(list[0].GetName(), QueryParams.Ping);
        }

    }
}
