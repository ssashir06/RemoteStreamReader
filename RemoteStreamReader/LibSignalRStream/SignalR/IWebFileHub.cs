using System;
namespace SignalRStream.SignalR
{
    public interface IWebFileHub
    {
        void Hello(string identifier);
        void FileOpened();
        void FileClosed();

        #region for stream
        
        void TellLength(string guid, long length);
        void TellBuffer(string guid, long begin, long end, string response);

        #endregion

    }
}
