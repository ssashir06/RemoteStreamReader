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

        void SendMessage(WebFileHubManagerSingleton hubManager, string connectionId, string message)
        {
            dynamic data = new ExpandoObject();
            data.text = message;
            hubManager.SendExtraData(connectionId, data);
        }

        public ActionResult Index()
        {
            var identifier = Guid.NewGuid().ToString();
            ViewBag.Identifier = identifier;

            Task.Factory.StartNew(async () =>
            {
                var hubManager = WebFileHubManagerSingleton.Instance;

                var connectionId = await hubManager.GetConnectionIdBy(identifier);
                Trace.WriteLine(string.Format("Connected. ConnectionId={0}", connectionId));
                SendMessage(hubManager, connectionId, "connected. wainting for opening file");


                while (!hubManager.IsOpened(connectionId))
                {
                    Thread.Sleep(1000);
                }
                Trace.WriteLine(string.Format("File opened"));
                SendMessage(hubManager, connectionId, "ok, ready to request file data.");

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
                SendMessage(hubManager, connectionId, sb.ToString());

            });

            return View();
        }

        public ActionResult SevenZipFileList()
        {
            var identifier = Guid.NewGuid().ToString();
            ViewBag.Identifier = identifier;

            Task.Factory.StartNew(async () =>
            {
                var hubManager = WebFileHubManagerSingleton.Instance;

                var connectionId = await hubManager.GetConnectionIdBy(identifier);
                Trace.WriteLine(string.Format("Connected. ConnectionId={0}", connectionId));
                SendMessage(hubManager, connectionId, "connected. wainting for opening file");

                while (!hubManager.IsOpened(connectionId))
                {
                    Thread.Sleep(1000);
                }
                Trace.WriteLine(string.Format("File opened"));
                SendMessage(hubManager, connectionId, "ok, ready to request file data.");

                try
                {
                    var sb = new StringBuilder();
                    using (var rws = new RemoteWebStream(connectionId))
                    using (var bfs = new BufferedStream(rws, 128))
                    using (var sz = new SevenZip.SevenZipExtractor(bfs))
                    {
                        sb.AppendFormat("file count = {1}{0}", Environment.NewLine, sz.FilesCount);
                        var files = sz.ArchiveFileNames;
                        foreach (var v in files.Select((name, idx) => new { Name = name, Idx = idx }))
                        {
                            sb.AppendFormat("FILE {1}: {2}{0}", Environment.NewLine, v.Idx, v.Name);
                        }
                    }
                    sb.AppendLine("DONE");
                    SendMessage(hubManager, connectionId, sb.ToString());
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    SendMessage(hubManager, connectionId, string.Format("Error: {1}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace));
                }
            });

            return View();
        }
    }
}