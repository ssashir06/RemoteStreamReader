using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRStream.SignalR
{
    [Flags]
    public enum ClientState
    {
        Disconnected = 0x00,
        Connected = 0x01,
        Opened = 0x02 | Connected,
    };
}
