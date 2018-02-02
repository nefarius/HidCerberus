using System;
using System.Collections.Generic;
using System.Linq;
using HidCerberus.Core;
using HidCerberus.Util;
using JetBrains.Annotations;
using Microsoft.Win32;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;

namespace HidCerberus.Srv.NancyFx.Modules
{
    [UsedImplicitly]
    public class HidGuardianNancyModuleV2 : NancyModule
    {
        private readonly JsonSerializerSettings _serializerSettings =
            new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error };

        public HidGuardianNancyModuleV2() : base("/api/v2")
        {
            #region Config

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

            #endregion

            #region Affected

            Get["/guardian/affected"] = _ =>
            {
                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase))
                {
                    var affected = wlKey?.GetValue("AffectedDevices") as string[];

                    return Response.AsJson(affected?.Select(a => new HidGuardianAffectedDevice { HardwareId = a }));
                }
            };

            Put["/guardian/affected"] = _ =>
            {
                var hwIds = JsonConvert.DeserializeObject<List<HidGuardianAffectedDevice>>(Request.Body.AsString());

                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase, true))
                {
                    var newList = new List<HidGuardianAffectedDevice>(hwIds);

                    // get existing Hardware IDs
                    var affected = (wlKey?.GetValue("AffectedDevices") as string[])
                        ?.Select(a => new HidGuardianAffectedDevice { HardwareId = a }).ToList();

                    // fuse arrays
                    if (affected != null)
                        newList.AddRange(affected);

                    // write back to registry
                    wlKey?.SetValue("AffectedDevices",
                        newList.Where(s => !string.IsNullOrWhiteSpace(s.HardwareId)).Distinct()
                            .Select(o => o.HardwareId)
                            .ToArray(),
                        RegistryValueKind.MultiString);
                }

                return Response.AsJson(hwIds, HttpStatusCode.Created);
            };

            Delete["/guardian/affected/{entityId}"] = parameters =>
            {
                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase, true))
                {
                    // get existing Hardware IDs
                    var affected = (wlKey?.GetValue("AffectedDevices") as string[])
                        ?.Select(a => new HidGuardianAffectedDevice { HardwareId = a }).ToList();

                    if (affected.RemoveAll(a => a.EntityId == parameters.entityId) <= 0)
                        throw new KeyNotFoundException();

                    // write back to registry
                    wlKey?.SetValue("AffectedDevices",
                        affected.Distinct()
                            .Select(o => o.HardwareId)
                            .ToArray(),
                        RegistryValueKind.MultiString);

                    return Response.AsJson(affected);
                }
            };

            #endregion

            #region Exempted

            Get["/guardian/exempted"] = _ =>
            {
                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase))
                {
                    var exempted = wlKey?.GetValue("ExemptedDevices") as string[];

                    return Response.AsJson(exempted?.Select(a => new HidGuardianAffectedDevice { HardwareId = a }));
                }
            };

            Put["/guardian/exempted"] = _ =>
            {
                var hwIds = JsonConvert.DeserializeObject<List<HidGuardianAffectedDevice>>(Request.Body.AsString());

                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase, true))
                {
                    // get existing Hardware IDs
                    var exempted = (wlKey?.GetValue("ExemptedDevices") as string[])
                        ?.Select(a => new HidGuardianAffectedDevice { HardwareId = a }).ToList();

                    // fuse arrays
                    if (exempted != null)
                        hwIds.AddRange(exempted);

                    // write back to registry
                    wlKey?.SetValue("ExemptedDevices",
                        hwIds.Where(s => !string.IsNullOrWhiteSpace(s.HardwareId)).Distinct().Select(o => o.HardwareId)
                            .ToArray(),
                        RegistryValueKind.MultiString);
                }

                return Response.AsJson(hwIds, HttpStatusCode.Created);
            };

            Delete["/guardian/exempted/{entityId}"] = parameters =>
            {
                using (var wlKey = Registry.LocalMachine.OpenSubKey(HidGuardianRegistryKeyBase, true))
                {
                    // get existing Hardware IDs
                    var exempted = (wlKey?.GetValue("ExemptedDevices") as string[])
                        ?.Select(a => new HidGuardianAffectedDevice { HardwareId = a }).ToList();

                    if (exempted.RemoveAll(a => a.EntityId == parameters.entityId) <= 0)
                        throw new KeyNotFoundException();

                    // write back to registry
                    wlKey?.SetValue("ExemptedDevices",
                        exempted.Distinct()
                            .Select(o => o.HardwareId)
                            .ToArray(),
                        RegistryValueKind.MultiString);

                    return Response.AsJson(exempted);
                }
            };

            #endregion
        }

        private static string HidGuardianRegistryKeyBase => @"SYSTEM\CurrentControlSet\Services\HidGuardian\Parameters";
    }
}