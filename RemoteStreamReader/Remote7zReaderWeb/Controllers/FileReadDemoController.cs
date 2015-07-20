using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Diagnostics;
using SignalRStream.SignalR;
using SignalRStream.Streams;
using System.Dynamic;

namespace Remote7zReaderWeb.Controllers
{
    public class FileReadDemoController : Controller
    {
        public ActionResult Index()
        {
            var identifier = Guid.NewGuid().ToString();
            ViewBag.Identifier = identifier;

            Task.Factory.StartNew(async () =>
            {
                var hubManager = WebFileHubManagerSingleton.Instance;

                var connectionId = await hubManager.GetConnectionIdBy(identifier);
                Trace.WriteLine(string.Format("Connected. ConnectionId={0}", connectionId));

                {
                    dynamic data = new ExpandoObject();
                    data.text = "connected.";
                    hubManager.SendExtraData(connectionId, data);
                }

                while (!hubManager.IsOpened(connectionId))
                {
                    Thread.Sleep(1000);
                }
                Trace.WriteLine(string.Format("File opened"));
                {
                    dynamic data = new ExpandoObject();
                    data.text = "ok, opened.";
                    hubManager.SendExtraData(connectionId, data);
                }

                var sb = new StringBuilder();
                sb.AppendLine("data read:");
                using (var rws = new RemoteWebStream(connectionId))
                using (var sr = new StreamReader(rws, Encoding.UTF8))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        sb.AppendLine(line);
                        Trace.WriteLine(line);
                    }

                    sb.AppendLine();
                    sb.AppendFormat("file size: {1}{0}", Environment.NewLine, rws.Length);
                }
                sb.AppendLine("DONE");

                {
                    dynamic data = new ExpandoObject();
                    data.text = sb.ToString();
                    hubManager.SendExtraData(connectionId, data);
                }

            });

            return View();
        }
    }
}