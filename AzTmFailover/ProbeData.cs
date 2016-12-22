using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace AzTmFailover
{
    public class ProbeEvent
    {
        [JsonProperty("callerip")]
        public string callerIp { get; set; }
        [JsonProperty("timeutc")]
        public DateTime timeUtc { get; set; }
        [JsonProperty("responsetimems")]
        public long responseTimeMs { get; set; }
        [JsonProperty("statuscode")]
        public int statusCode { get; set; }
        [JsonProperty("fullcheck")]
        public bool fullCheck { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
    }

    public class ProbeData
    {
        [JsonProperty("webserver")]
        public string webServer { get; set; }

        [JsonProperty("statuscode")]
        public int statusCode { get; set; }

        [JsonProperty("lastfullhealtcheckutc")]
        public DateTime lastFullHealthCheckUtc { get; set; }

        [JsonProperty("probes")]
        public ProbeEvent[] probes { get; set; }
    }
}