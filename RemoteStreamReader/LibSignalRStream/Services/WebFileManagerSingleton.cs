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
    public class WebFileHubManagerSingleton
    {
        WebFileHubManagerSingleton() { }

        #region Singleton Instance
        
        static Lazy<WebFileHubManagerSingleton> _instance = new Lazy<WebFileHubManagerSingleton>(() => new WebFileHubManagerSingleton());

        public static WebFileHubManagerSingleton Instance { get { return _instance.Value; } }

        #endregion

        #region Hello

        class HelloSemapher : SemaphoreSlim
        {
            public HelloSemapher() : base(0, 1) { }
            public string ConnectionId { get; set; }
        }

        static IDictionary<string, HelloSemapher> _helloWaits = new Dictionary<string, HelloSemapher>();

        public void Hello(string connectionId, string identifier)
        {
            lock (_helloWaits)
            {
                if (!_helloWaits.ContainsKey(identifier))
                {
                    throw new ArgumentException("The identifier is not expected");
                }

                HelloSemapher semapher;
                lock (_helloWaits)
                {
                    semapher = _helloWaits[identifier];
                }
                semapher.ConnectionId = connectionId;
                semapher.Release();
            }
        }

        public async Task<string> GetConnectionIdBy(string identifier)
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

        #region FileBody

        static AsyncRequestAbstract<string> _filebodyRequests = new AsyncRequestAbstract<string>();

        public void SetSignalrValueFileData(string connectionId, string guid, string response)
        {
            _filebodyRequests.SetSignalRValue(connectionId, guid, response);
        }
        
        public async Task<string> GetFileData(string connectionId, int begin, int end)
        {
            return await _filebodyRequests.GetRequestResult(connectionId, (guid, client) =>
                client.GetFileData(guid.ToString(), begin, end)
            );
        }

        #endregion

    }
}
