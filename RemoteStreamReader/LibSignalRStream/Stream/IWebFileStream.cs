using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SignalRStream.Stream
{
    public interface IWebFileStream : IDisposable
    {
        string Request(long begin, long end);
    }
}
