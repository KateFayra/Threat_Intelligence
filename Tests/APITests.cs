using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using ResourceLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class APITests
    {
        string ip = "54.186.91.102";
        string domain = "njlochner.com";

        [TestMethod()]
        public async Task APITest()
        {
            Thread hostthread = new Thread(() => ServiceInfoGatherer.Program.Main(null));
            Thread worker1 = new Thread(() => Worker.Program.Main(new string[] { "10001" }));
            Thread worker2 = new Thread(() => Worker.Program.Main(new string[] { "10002" }));
            hostthread.Start();
            worker1.Start();
            worker2.Start();

            Thread.Sleep(1000);

            JObject result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/domain/" + domain);

            Assert.AreEqual(result["Whois"]["WhoisRecord"]["domainName"], domain);
            Assert.AreEqual(result["RDAP"]["ldhName"], domain.ToUpper());
            Assert.AreEqual(result["DNS"]["ips"]["0"], ip);



            result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/ip/" + ip);


            Assert.AreEqual(result["AbuseIPDB"]["data"]["ipAddress"], ip);
            Assert.AreEqual(result["ReverseDNS"]["hostname"], "ec2-54-186-91-102.us-west-2.compute.amazonaws.com");
            Assert.AreEqual(result["GeoIP"]["status"], "success");
            Assert.AreEqual(result["Ping"]["Status"], "TimedOut");
            Assert.AreEqual(result["Shodan"], null);


            result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/ip/" + ip + "?Shodan=true");


            Assert.AreEqual(result["AbuseIPDB"], null);
            Assert.AreEqual(result["ReverseDNS"], null);
            Assert.AreEqual(result["GeoIP"], null);
            Assert.AreEqual(result["Ping"], null);
            Assert.AreEqual(result["Shodan"]["ip_str"], ip);


            result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/domain/" + domain + "?DNS=true&RDAP=true");

            Assert.AreEqual(result["Whois"], null);
            Assert.AreEqual(result["RDAP"]["ldhName"], domain.ToUpper());
            Assert.AreEqual(result["DNS"]["ips"]["0"], ip);


            result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/domain/" + domain + "?Foobar");
            Assert.AreEqual(result["Error"], "Response status code does not indicate success: 400 (Bad Request).");


            result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/domain/" + domain + "?DNS=false");
            Assert.AreEqual(result["Error"], "Response status code does not indicate success: 400 (Bad Request).");


            result = await HTTPMethods.JSONQuery("http://127.0.0.1:8080/domain/" + domain + "?DNS=false&RDAP=true");
            Assert.AreEqual(result["Whois"], null);
            Assert.AreEqual(result["RDAP"]["ldhName"], domain.ToUpper());
            Assert.AreEqual(result["DNS"], null);

        }
    }
}