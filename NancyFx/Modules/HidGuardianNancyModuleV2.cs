using System;
using System.Linq;
using HidCerberus.Srv.Core;
using HidCerberus.Srv.Util;
using Microsoft.Win32;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace HidCerberus.Srv.NancyFx.Modules
{
    public class HidGuardianNancyModuleV2 : NancyModule
    {
        private readonly JsonSerializerSettings _serializerSettings =
            new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Error};

        public HidGuardianNancyModuleV2() : base("/api/v2")
        {
            Get["/guardian/config"] = _ =>
            {
                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase))
                {
                    return Response.AsJson(new HidGuardianConfiguration
                    {
                        AllowByDefault = Convert.ToBoolean(wlKey?.GetValue("AllowByDefault")),
                        Force = Convert.ToBoolean(wlKey?.GetValue("Force"))
                    });
                }
            };

            Put["/guardian/config"] = _ =>
            {
                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase, true))
                {
                    var json = Request.Body.AsString();
                    var cfg = JsonConvert.DeserializeObject<HidGuardianConfiguration>(json, _serializerSettings);

                    wlKey?.SetNonNullValueDword("AllowByDefault", cfg.AllowByDefault);
                    wlKey?.SetNonNullValueDword("Force", cfg.Force);

                    return Response.AsText(json);
                }
            };

            Get["/guardian/affected"] = _ =>
            {
                var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase);
                var affected = wlKey?.GetValue("AffectedDevices") as string[];
                wlKey?.Close();

                return Response.AsJson(affected?.Select(a => new {hardwareId = a}));
            };
        }

        private static string HidGuardianRegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters";
    }
}