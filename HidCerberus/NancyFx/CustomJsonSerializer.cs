using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HidCerberus.Srv.NancyFx
{
    public class CustomJsonSerializer : JsonSerializer
    {
        public CustomJsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Formatting = Formatting.Indented;
        }
    }
}