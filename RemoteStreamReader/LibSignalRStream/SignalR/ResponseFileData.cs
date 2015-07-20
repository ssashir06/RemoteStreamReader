using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRStream.SignalR
{
    public class ResponseFileData
    {

        readonly Lazy<byte[]> _decoded;

        public ResponseFileData(long begin, long end, string dataEncodedByBase64)
        {
            Begin = begin;
            End = end;
            DataBase64 = dataEncodedByBase64;
            _decoded = new Lazy<byte[]>(() => Convert.FromBase64String(DataBase64));
        }

        public long Begin { get; protected set; }
        public long End { get; protected set; }
        public string DataBase64 { get; protected set; }
        public byte[] DataDecoded
        {
            get { return _decoded.Value; }
        }
    }
}
