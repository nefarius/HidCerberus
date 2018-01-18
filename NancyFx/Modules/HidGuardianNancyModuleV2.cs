using System.Collections.Generic;
using Microsoft.Win32;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json.Linq;

namespace HidCerberus.Srv.NancyFx.Modules
{
    public class HidGuardianNancyModuleV2 : NancyModule
    {
        public HidGuardianNancyModuleV2() : base("/api/v2")
        {
            Get["/guardian/force"] = _ =>
            {
                var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase);
                var force = wlKey?.GetValue("Force");
                wlKey?.Close();

                return Response.AsJson(new { force });
            };

            Post["/guardian/force"] = parameters =>
            {
                dynamic json = JObject.Parse(Request.Body.AsString());

                var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase, true);
                wlKey?.SetValue("Force", (int)json.force);
                wlKey?.Close();

                return Response.AsJson(new { json.force });
            };
        }

        private static string HidGuardianRegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters";
    }
}