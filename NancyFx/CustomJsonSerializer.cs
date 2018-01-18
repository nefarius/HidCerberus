using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HidCerberus.Srv.NancyFx
{
    public class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            this.ContractResolver = new CamelCasePropertyNamesContractResolver();
            this.Formatting = Formatting.Indented;
        }
    }
}