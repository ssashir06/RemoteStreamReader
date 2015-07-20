using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            WebFileHubManagerSingleton.Instance.Hello(Context.ConnectionId, identifier);
        }

        public void FileOpened()
        {
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Opened);
        }

        public void FileClosed()
        {
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Connected);
        }

        public void Bye()
        {
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Disconnected);
        }

        public void TellLength(string guid, long length)
        {
            WebFileHubManagerSingleton.Instance.SetSignalrValueFileSize(Context.ConnectionId, guid, length);
        }

        public void TellBuffer(string guid, long begin, long end, string response)
        {
            WebFileHubManagerSingleton.Instance.SetSignalrValueFileData(Context.ConnectionId, guid, begin, end, response);
        }

        #endregion

        #region Output

        public static void GetFileSize(Guid guid, dynamic client)
        {
            client.GetFileSize(guid.ToString());
        }

        public static void GetFileData(Guid guid, dynamic client, long begin, long end)
        {
            client.GetFileData(guid.ToString(), begin, end);
        }
        
        #endregion

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            WebFileHubManagerSingleton.Instance.SetClientState(Context.ConnectionId, ClientState.Disconnected);
            return base.OnDisconnected(stopCalled);
        }

    }
}
