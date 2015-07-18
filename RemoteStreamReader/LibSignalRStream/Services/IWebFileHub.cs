using System;
namespace SignalRStream.Services
{
    public interface IWebFileHub
    {
        void Hello(string identifier);
        void Response(string guid, int begin, int end, string response);
    }
}
