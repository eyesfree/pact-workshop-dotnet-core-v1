using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace tests
{
    internal class TestResponse
    {
        [JsonProperty("test")]
        public string Test { get; set; }
        [JsonProperty("validDateTime")]
        public string ValidDateTime { get; set; }
    }
}