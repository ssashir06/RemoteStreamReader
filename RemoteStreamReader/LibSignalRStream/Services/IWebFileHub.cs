using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRStream.Services
{
    public interface IWebFileHubManager
    {
        Task<string> Request(string connectionId, int begin, int end);
        Task<string> GetConnectionIdBy(string identifier);
    }
}
