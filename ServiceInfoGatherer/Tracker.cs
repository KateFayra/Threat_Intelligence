using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace ServiceInfoGatherer
{
    public class Tracker
    {
        public int numQueries = -1;
        public int finishedQueries = 0;
        public HttpListenerContext context;
        public List<JObject> responses = new List<JObject>();

        public Tracker(HttpListenerContext context)
        {
            this.context = context;
        }
    }
}
