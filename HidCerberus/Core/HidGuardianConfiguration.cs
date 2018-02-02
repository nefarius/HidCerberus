using System.ComponentModel;
using Newtonsoft.Json;

namespace HidCerberus.Core
{
    public class HidGuardianConfiguration
    {
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool? Force { get; set; }

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool? AllowByDefault { get; set; }
    }
}
