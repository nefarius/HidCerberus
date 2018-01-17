using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace HidCerberus.Srv.Core
{
    public class CerberusPluginHost
    {
        private static readonly string AccessProvidersPath = Path.Combine(Path.GetDirectoryName
            (Assembly.GetExecutingAssembly().Location), "AccessProviders");

        [ImportMany]
        private Lazy<IAccessProvider, IDictionary<string, object>>[] AccessProviders { get; set; }

        public CerberusPluginHost()
        {
            //Creating an instance of aggregate catalog. It aggregates other catalogs
            var aggregateCatalog = new AggregateCatalog();

            //Load parts from the current assembly if available
            var asmCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            //Add to the aggregate catalog
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(AccessProvidersPath, "*.dll"));
            aggregateCatalog.Catalogs.Add(asmCatalog);

            //Crete the composition container
            var container = new CompositionContainer(aggregateCatalog);

            // Composable parts are created here i.e. 
            // the Import and Export components assembles here
            container.ComposeParts(this);
        }
    }
}