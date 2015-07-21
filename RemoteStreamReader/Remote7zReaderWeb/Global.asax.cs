using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.IO;
using System.Reflection;

namespace Remote7zReaderWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundle(BundleTable.Bundles);

            SetupSevenZipSharpDll();
        }

        void SetupSevenZipSharpDll()
        {
            Util.SevenZipSharpUtil.InitDll(
                HostingEnvironment.MapPath("~/App_Data/dll/7za.dll"), 
                HostingEnvironment.MapPath("~/App_Data/dll/7z64.dll"));
        }
    }
}
