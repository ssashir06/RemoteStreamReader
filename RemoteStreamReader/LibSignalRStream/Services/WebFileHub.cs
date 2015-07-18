using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRStream.Services
{
    [HubName("WebFileStream")]
    public class WebFileHub : Hub
    {
        public WebFileHub() { }

        #region SignalR Methods

        public void Response(string guid, string response)
        {
            WebFileHubManagerSingleton.Instance.Response(Context.ConnectionId, guid, response);
        }

        public void Hello(string identifier)
        {
            WebFileHubManagerSingleton.Instance.Hello(Context.ConnectionId, identifier);
        }

        #endregion
    }
}
