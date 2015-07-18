using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace SignalRStream.Services
{
    public class WebFileHubManagerSingleton : IWebFileHubManager
    {
        class ResponseContainer : SemaphoreSlim
        {
            public ResponseContainer()
                : base(0, 1)
            {
                Guid = Guid.NewGuid();
            }

            public Guid Guid { get; protected set; }
            public string Response { get; set; }
        }

        class HelloSemapher : SemaphoreSlim
        {
            public HelloSemapher() : base(0, 1) { }
            public string ConnectionId { get; set; }
        }

        static Lazy<WebFileHubManagerSingleton> _instance = new Lazy<WebFileHubManagerSingleton>(() => new WebFileHubManagerSingleton());
        static IDictionary<string, IList<ResponseContainer>> _responses = new Dictionary<string, IList<ResponseContainer>>();
        static IDictionary<string, HelloSemapher> _helloWaits = new Dictionary<string, HelloSemapher>();

        WebFileHubManagerSingleton() { }

        public static WebFileHubManagerSingleton Instance { get { return _instance.Value; } }

        #region SignalR Methods

        public void Response(string connectionId, string guid, string response)
        {
            Trace.TraceInformation(string.Format("Response {0}: {1}", connectionId, response));

            IList<ResponseContainer> containers = null;
            lock (_responses)
            {
                if (!_responses.ContainsKey(connectionId))
                {
                    throw new Exception("Unexpected Connection Id");
                }

                containers = _responses[connectionId];
            }

            ResponseContainer container = null;
            lock (containers)
            {
                container = (
                    from c in containers 
                    where c.Guid.ToString() == guid 
                    select c
                    ).FirstOrDefault();
            }

            if (container == null)
            {
                throw new ArgumentException("Unexpected GUID");
            }

            container.Response = response;
            container.Release();
        }

        public void Hello(string connectionId, string identifier)
        {
            lock (_helloWaits)
            {
                if (!_helloWaits.ContainsKey(identifier))
                {
                    throw new ArgumentException("The identifier is not expected");
                }

                var semapher = _helloWaits[identifier];
                semapher.ConnectionId = connectionId;
                semapher.Release();
            }
        }
        
        #endregion

        #region IWebFileHub メンバー

        async Task<string> IWebFileHubManager.Request(string connectionId, int begin, int end)
        {
            IList<ResponseContainer> containers = null;
            lock (_responses)
            {
                if (!_responses.ContainsKey(connectionId))
                {
                    _responses.Add(connectionId, containers = new List<ResponseContainer>());
                }
                else
                {
                    containers = _responses[connectionId];
                }
            }

            string response;
            using (var container = new ResponseContainer())
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<WebFileHub>();
                lock (containers)
                {
                    containers.Add(container);
                }
                context.Clients.Client(connectionId).RequestRange(container.Guid.ToString(), begin, end);

                await container.WaitAsync();
                response = container.Response;
                // TODO: タイムアウトの取り扱い

                lock (containers)
                {
                    containers.Remove(container);
                }
            }

            return response;
        }


        async Task<string> IWebFileHubManager.GetConnectionIdBy(string identifier)
        {
            using (var semapher = new HelloSemapher())
            {
                lock (_helloWaits)
                {
                    if (_helloWaits.ContainsKey(identifier))
                    {
                        throw new ArgumentException("The identifier is aleady started");
                    }
                    _helloWaits.Add(identifier, semapher);
                }
                await semapher.WaitAsync();
                lock (_helloWaits)
                {
                    _helloWaits.Remove(identifier);
                }
                return semapher.ConnectionId;
            }
        }

        #endregion

    }
}
