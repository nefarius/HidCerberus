using JetBrains.Annotations;
using Nancy;

namespace HidCerberus.NancyFx.Modules
{
    [UsedImplicitly]
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = _ => View["index"];
        }
    }
}
