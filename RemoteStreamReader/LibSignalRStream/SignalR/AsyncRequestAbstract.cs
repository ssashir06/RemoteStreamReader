using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalRStream.SignalR
{
    class AsyncRequestAbstract<TResponse>
    {
        #region class

        public class TooManyRequests : Exception
        {
            public TooManyRequests() : base() { }
            public TooManyRequests(string message) : base(message) { }
        }


        class ValueContainerSemaphore<TValue> : SemaphoreSlim
        {
            public ValueContainerSemaphore()
                : base(0, 1)
            {
                Guid = Guid.NewGuid();
            }

            public TValue Value { get; set; }
            public Guid Guid { get; protected set; }
        }

        #endregion

        readonly int? _maxRequestPerConnection;
        readonly IDictionary<string, IList<ValueContainerSemaphore<TResponse>>> _responses = new Dictionary<string, IList<ValueContainerSemaphore<TResponse>>>();

        public AsyncRequestAbstract(int? numberOfMaxRequestsPerConnection = null)
        {
            _maxRequestPerConnection = numberOfMaxRequestsPerConnection;
        }

        public async Task<TResponse> GetRequestResult(string connectionId, Action<Guid, dynamic> signalrCaller)
        {
            IList<ValueContainerSemaphore<TResponse>> containers;
            lock (_responses)
            {
                if (!_responses.ContainsKey(connectionId))
                {
                    _responses.Add(connectionId, containers = new List<ValueContainerSemaphore<TResponse>>());
                }
                else
                {
                    containers = _responses[connectionId];
                }
            }

            lock (containers)
            {
                if (_maxRequestPerConnection.HasValue && containers.Count - 1 >= _maxRequestPerConnection)
                {
                    throw new TooManyRequests("Too many requests");
                }
            }

            TResponse response;

            using (var container = new ValueContainerSemaphore<TResponse>())
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<WebFileHub>();
                lock (containers)
                {
                    containers.Add(container);
                }
                signalrCaller(container.Guid, context.Clients.Client(connectionId));

                await container.WaitAsync();
                response = container.Value;
                // TODO: タイムアウトの取り扱い

                lock (containers)
                {
                    containers.Remove(container);
                }
            }

            return response;
        }

        public void SetSignalRValue(string connectionId, string guid, TResponse value)
        {
            IList<ValueContainerSemaphore<TResponse>> containers;
            lock (_responses)
            {
                if (!_responses.ContainsKey(connectionId))
                {
                    throw new Exception("Unexpected Connection Id");
                }

                containers = _responses[connectionId];
            }

            ValueContainerSemaphore<TResponse> container;
            Guid guidReceived = Guid.Parse(guid);
            lock (containers)
            {
                container = (
                    from c in containers 
                    where c.Guid == guidReceived
                    select c
                    ).FirstOrDefault();
            }

            if (container == null)
            {
                throw new ArgumentException("Unexpected GUID");
            }

            container.Value = value;
            container.Release();
        }
    }
}
