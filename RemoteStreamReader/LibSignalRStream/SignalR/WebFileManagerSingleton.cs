using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;

namespace SignalRStream.SignalR
{
    public class WebFileHubManagerSingleton
    {
        WebFileHubManagerSingleton() { }

        #region Singleton Instance
        
        static Lazy<WebFileHubManagerSingleton> _instance = new Lazy<WebFileHubManagerSingleton>(() => new WebFileHubManagerSingleton());

        public static WebFileHubManagerSingleton Instance { get { return _instance.Value; } }

        #endregion

        #region Ready?

        readonly IDictionary<string, ClientState> _clientStates = new Dictionary<string, ClientState>();

        public bool IsConnected(string connectionId)
        {
            lock (_clientStates)
            {
                if (!_clientStates.ContainsKey(connectionId))
                {
                    return false;
                }

                return _clientStates[connectionId].HasFlag(ClientState.Connected);
            }
        }

        public bool IsOpened(string connectionId)
        {
            lock (_clientStates)
            {
                if (!_clientStates.ContainsKey(connectionId))
                {
                    return false;
                }

                return _clientStates[connectionId].HasFlag(ClientState.Opened);
            }
        }

        public void SetClientState(string connectionId, ClientState state)
        {
            lock (_clientStates)
            {
                switch (state)
                {
                    case ClientState.Disconnected:
                        if (_clientStates.ContainsKey(connectionId))
                        {
                            _clientStates.Remove(connectionId);
                        }
                        return;
                    case ClientState.Opened:
                    case ClientState.Connected:
                        _clientStates[connectionId] = state;
                        return;
                    default:
                        throw new ArgumentException("Bad state");
                }
            }
        }

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

                SetClientState(connectionId, ClientState.Connected);
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

        #region get Length

        static AsyncRequestAbstract<long> _fileSizeRequests = new AsyncRequestAbstract<long>();

        public void SetSignalrValueFileSize(string connectionId, string guid, long length)
        {
            _fileSizeRequests.SetSignalRValue(connectionId, guid, length);
        }

        public async Task<long> GetFileSize(string connectionId)
        {
            return await _fileSizeRequests.GetRequestResult(connectionId, (guid, client) =>
                WebFileHub.GetFileSize(guid, client)
            );
        }
        
        #endregion

        #region FileBody

        static AsyncRequestAbstract<string> _fileBodyRequests = new AsyncRequestAbstract<string>();

        public void SetSignalrValueFileData(string connectionId, string guid, string response)
        {
            _fileBodyRequests.SetSignalRValue(connectionId, guid, response);
        }
        
        public async Task<string> GetFileData(string connectionId, long begin, long end)
        {
            return await _fileBodyRequests.GetRequestResult(connectionId, (guid, client) =>
                WebFileHub.GetFileData(guid, client, begin, end)
            );
        }

        #endregion

    }
}
