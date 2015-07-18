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

        #region IWebFileHub メンバー

        public void Hello(string identifier)
        {
            WebFileHubManagerSingleton.Instance.Hello(Context.ConnectionId, identifier);
        }

        public void ResponseFileData(string guid, int begin, int end, string response)
        {
            WebFileHubManagerSingleton.Instance.SetSignalrValueFileData(Context.ConnectionId, guid, response);
        }

        #endregion
    }
}
