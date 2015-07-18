using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRStream.SignalR
{
    class ValueContainerSemaphore<TValue>
        : SemaphoreSlim
    {
        public ValueContainerSemaphore() : base(0, 1)
        {
            Guid = Guid.NewGuid();
        }

        public TValue Value { get; set; }
        public Guid Guid { get; protected set; }
    }
}
