using JetBrains.Annotations;
using Nancy;

namespace HidCerberus.Srv.NancyFx.Modules
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
