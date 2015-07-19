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
    public class RemoteWebStream : Stream
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
            string receiveBase64Encoded = WebFileHubManagerSingleton.Instance.GetFileData(ConnectionId, Position, Position + count).Result;
            var receiveDecoded = Convert.FromBase64String(receiveBase64Encoded);
            int copySize = Math.Min(count, receiveDecoded.Count());
            Buffer.BlockCopy(receiveDecoded, 0, buffer, offset, copySize);
            Trace.WriteLineIf(count > receiveDecoded.Count(), "Received size is too large.");
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
                    p = Length - 1;
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
    }
}
