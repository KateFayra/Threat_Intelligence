# Threat Intelligence

An API which provides information for given IP addresses and domain names. Designed to be used to provide intelligence as part of a security incident response process. A demo is currently running at http://threatintel.njlochner.com (IP: http://54.234.211.140/)

## Features

Currently provides the following information:
####  IP addresses:
- GeoIP data (latitude, longitude, city, country, ISP)
- Reverse DNS Lookup
- Ping
- AbuseIPDB.com data (confidence score, number of reports, distinct user reports, whitelist information)
- Shodan.io data (open ports, running services, software hosting services, software versions, operating system information, HTTP responses, SSL certificate info)

####  Domain names:
- DNS Lookup
- RDAP registry info
- Whois registry info

## Usage
The API provides two endpoints:
| Request Type               |Path|Optional Parameters |
|----------------|-------------------------------|-----------------------------|
|GET|    /ip/{ipAddress}     |GeoIP=true, ReverseDNS=true, Ping=true, AbuseIPDB=true, Shodan=true|
|GET|  /domain/{domainName}  |DNS=true, RDAP=true, Whois=true           |

###  /ip/{ipAddress}

Example call: GET http://threatintel.njlochner.com/ip/54.186.91.102
If no parameters are specified, GeoIP, ReverseDNS, Ping, and AbuseIPDB.com information will be returned. The response is provided as JSON. 

Shodan.io data is not provided by default due Shodan's API having an occasional delay of approx 15 seconds. To retrieve Shodan data, the 'Shodan=true' parameter must be specified. E.g. http://threatintel.njlochner.com/ip/54.186.91.102?Shodan=true 

Note that when using the optional parameters, only information for the specified parameters will be shown. For example, in order to retrieve Shodan data and AbuseIPDB data, `?Shodan=true&AbuseIPDB=true` must be specified.

#### Response Format

```
{
"GeoIP" : { ... },
"Ping":{...},
"ReverseDNS":{...},
"AbuseIPDB":{...},
"Shodan":{...}
}
```

With the following values:

- ReverseDNS
```
{
"hostname": ... 
}
```
- Ping
```
{
"Status": "Success", "TimedOut", etc
"RoundTripTime": milliseconds
"TimeToLive": ...
"NumBytes": Number of bytes received in reply
}
```
- GeoIP (See https://ip-api.com/docs/api:json)
-  AbuseIPDB.com (See https://docs.abuseipdb.com/?shell#check-endpoint)
- Shodan.io (See the `/shodan/host/{ip}`section on https://developer.shodan.io/api)

### /domain/{domainName}
Example call: GET http://threatintel.njlochner.com/domain/njlochner.com
If no parameters are specified, DNS, RDAP, and Whois information will be returned. The response is provided as JSON. 

#### Response Format
```
{
"DNS" : { ... },
"RDAP":{...},
"Whois":{...}
}
```
	
With the following values:

- DNS
```
{
"ips": {
 "0": IP address,
 "1": IP address,
...
 "N": IP address
}
}
```
    
- RDAP  (See https://www.verisign.com/en_US/domain-names/registration-data-access-protocol/index.xhtml)
- Whois (See https://whois.whoisxmlapi.com/documentation/making-requests)

## Implementation Details

This API was developed using C# .NET 5.0. The API server processes the request and determines which queries should run. Then it splits the queries evenly, and issues commands to multiple worker servers which can run on separate hosts. 

The workers perform the queries in parallel, then send the responses back to the API server after all queries have been finished. The API server then returns the JSON response back to the requesting client after all workers have returned responses. If one query fails on a worker (due to rate limiting or an error), successful queries from that worker will still be returned.

This project also contains unit tests which have 93% code coverage.

Note: If compiling from source, you will need to specify your API key for AbuseIPDB.com in AbuseIPDBQuery.cs, for Shodan.io in ShodanQuery.cs, and for whoisxmlapi.com in WhoisQuery.cs. The program will still function without the API keys, but you will be unable to retrieve results for those three queries.

#### Dependencies (NuGet)
- Newtonsoft.Json
- Nager.PublicSuffix


### Future Improvements
Additional features I would like to add:

- Reverse DNS should be able to provide multiple hostnames/domain names.
- If a DNS lookup is performed and IP addresses are obtained, there should be an option to automatically retrieve IP address query information without the client needing to make a second request. (Also similar functionality if a Reverse DNS lookup is performed and domains are retrieved from an IP address.)
- Allow queries for multiple IPs and domains simultaneously.
- Number of workers and worker hostnames/IPs/ports should be read from a config file.
- API keys should be read from a config file.
- Use authentication when communicating between API server and workers.
- Add an endpoint for binaries, and return scan results from Virustotal.
- Add an endpoint for URLs and use Google's safe browsing API https://developers.google.com/safe-browsing
- Check if an IP is likely to be a VPN/Proxy. Also check if likely to be a datacenter or residential.
- Improve IQuery Interface architecture to remove duplicate GetName() method in query classes, perhaps switching to use an Abstract class.
- Containerize using Docker.
- Develop a front-end web-application which can take in a list of IPs, domains, binaries, URLs to query and display results.

- Use mocking for some unit tests.
- Add additional test cases for specific scenarios.

### Other relevant APIs/resources to use: 

https://pulsedive.com/api/

https://github.com/hslatman/awesome-threat-intelligence

Proxy / VPN check https://www.ipqualityscore.com/ip-reputation-check

SSL Cert info - https://threatintelligenceplatform.com/ssl-certificate-chain-api
 
https://www.ripe.net/manage-ips-and-asns/db/support/querying-the-ripe-database

https://team-cymru.com/community-services/ip-asn-mapping/

https://attackerkb.com/

https://www.reddit.com/r/datasets/comments/7vj5eg/cybersecurity_datasets/

https://pentest-tools.com/website-vulnerability-scanning/website-scanner

https://snyk.io/website-scanner/

http://www.insecam.org/en/faq/

Greynoise

minemeld

#### IP blacklists:
https://zeltser.com/malicious-ip-blocklists/

https://whatismyipaddress.com/blacklist-check

https://www.threatstop.com/check-ioc

https://dnschecker.org/ip-blacklist-checker.php

https://avinetworks.com/docs/latest/ip-reputation/

https://ipremoval.sms.symantec.com/

https://www.projecthoneypot.org/list_of_ips.php



