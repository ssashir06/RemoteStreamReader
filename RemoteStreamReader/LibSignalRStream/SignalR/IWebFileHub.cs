using System;
namespace SignalRStream.SignalR
{
    public interface IWebFileHub
    {
        void Hello(string identifier);
        void ResponseFileData(string guid, int begin, int end, string response);
    }
}
