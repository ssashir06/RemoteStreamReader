using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using SignalRStream.Services;

namespace Remote7zReaderWeb.Controllers
{
    public class SignalRStreamDemoController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Identifier = Guid.NewGuid().ToString();
            return View();
        }

        public ActionResult ReadFile(string id)
        {
            ViewBag.Identifier = id;


            Task.Factory.StartNew(async () =>
            {
                var hubManager = WebFileHubManagerSingleton.Instance;

                var connectionid = await hubManager.GetConnectionIdBy(id);
                Trace.WriteLine(string.Format("Connected. ConnectionId={0}", connectionid));

                var response = await hubManager.Request(connectionid, 0, 100);
                Trace.WriteLine(string.Format("Response is {0}{1}", Environment.NewLine, response));
            });

            return View();
        }
    }
}