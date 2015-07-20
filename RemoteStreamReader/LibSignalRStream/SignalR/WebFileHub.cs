using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRStream.SignalR
{
    [HubName("WebFileStream")]
    public class WebFileHub : Hub<IWebFileHub>, IWebFileHub
    {
        public WebFileHub() { }

        #region IWebFileHub メンバー (Input)

        public void Hello(string identifier)
        {
            Trace.WriteLine(string.Format("Receive on {0}: identifier={1}", Context.ConnectionId, identifier));
            WebFileHubManagerSingleton.Instance.Hello(Context.ConnectionId, identifier);
        }

        public void FileOpened()
        {
            Trace.WriteLine(string.Format("Receive on {0}: file opened", Context.ConnectionId));
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Opened);
        }

        public void FileClosed()
        {
            Trace.WriteLine(string.Format("Receive on {0}: file closed", Context.ConnectionId));
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Connected);
        }

        public void Bye()
        {
            Trace.WriteLine(string.Format("Receive on {0}: bye", Context.ConnectionId));
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Disconnected);
        }

        public void TellLength(string guid, long length)
        {
            Trace.WriteLine(string.Format("Receive on {0}({1}): length={2}", Context.ConnectionId, guid, length));
            WebFileHubManagerSingleton.Instance.SetSignalrValueFileSize(Context.ConnectionId, guid, length);
        }

        public void TellBuffer(string guid, long begin, long end, string response)
        {
            Trace.WriteLine(string.Format("Receive on {0}({1}): begin={2}, end={3}, response.length={4}", Context.ConnectionId, guid, begin, end, response.Length));
            WebFileHubManagerSingleton.Instance.SetSignalrValueFileData(Context.ConnectionId, guid, begin, end, response);
        }

        #endregion

        #region Output

        public static void GetFileSize(Guid guid, dynamic client)
        {
            Trace.WriteLine(string.Format("Send on {0}({1}): get file size command", "NA", guid));
            client.GetFileSize(guid.ToString());
        }

        public static void GetFileData(Guid guid, dynamic client, long begin, long end)
        {
            Trace.WriteLine(string.Format("Send on {0}({1}): get file data command(begin={2}, end={3})", "NA", guid, begin, end));
            client.GetFileData(guid.ToString(), begin, end);
        }

        public static void CloseFile(dynamic client)
        {
            Trace.WriteLine(string.Format("Send on {0}({1}): close file command", "NA", "NA"));
            client.CloseFile();
        }

        public static void ReceiveExtraData(dynamic client, dynamic data)
        {
            Trace.WriteLine(string.Format("Send to {0}({1}): sending extra data", "NA", "NA"));
            client.ReceiveExtraData(data);
        }
        
        #endregion

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Disconnected);
            return base.OnDisconnected(stopCalled);
        }

    }
}
