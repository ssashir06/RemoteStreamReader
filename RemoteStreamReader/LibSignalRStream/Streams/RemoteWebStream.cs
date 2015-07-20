using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using SignalRStream.SignalR;

namespace SignalRStream.Streams
{
    public class RemoteWebStream : Stream, IDisposable
    {
        public RemoteWebStream(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId{ get; protected set; }

        #region Stream override methods

        #region Read methods

        long? _fileSize;

        public override bool CanRead
        {
            get { return WebFileHubManagerSingleton.Instance.IsOpened(ConnectionId); }
        }

        public override bool CanSeek
        {
            get { return WebFileHubManagerSingleton.Instance.IsOpened(ConnectionId); }
        }

        public override long Length
        {
            get
            {
                if (!_fileSize.HasValue)
                {
                    _fileSize = WebFileHubManagerSingleton.Instance.GetFileSize(ConnectionId).Result;
                }
                return _fileSize.Value;
            }
        }

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var receivedData = WebFileHubManagerSingleton.Instance.GetFileData(ConnectionId, Position, Position + count - 1).Result;
            int copySize = Math.Min(count, receivedData.DataDecoded.Count());
            Buffer.BlockCopy(receivedData.DataDecoded, 0, buffer, offset, copySize);
            Trace.WriteLineIf(count < receivedData.DataDecoded.Count(), "Received size is too large.");
            Position += copySize;
            return copySize;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long p;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    p = 0;
                    break;
                case SeekOrigin.Current:
                    p = Position;
                    break;
                case SeekOrigin.End:
                    //p = Length - 1; Doesn't work
                    p = Length;
                    break;
                default:
                    throw new ArgumentException();
            }
            return Position = p + offset;
        }
        
        #endregion

        #region Write methods (unsupported features)
        
        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
        
        #endregion

        #region IDisposable メンバー

        void IDisposable.Dispose()
        {
            WebFileHubManagerSingleton.Instance.CloseFile(ConnectionId);
        }

        #endregion
    }
}
