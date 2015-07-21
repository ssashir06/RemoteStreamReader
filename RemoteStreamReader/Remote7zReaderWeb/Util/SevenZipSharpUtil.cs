using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SevenZip;

namespace Remote7zReaderWeb.Util
{
    public static class SevenZipSharpUtil
    {
        public static void InitDll(string dll32bit, string dll64bit)
        {
            string dll;
            switch (IntPtr.Size)
            {
                case 4:// 32bit
                    dll = dll32bit;
                    break;
                case 8:// 64bit
                    dll = dll64bit;
                    break;
                default:
                    throw new Exception("Cannot determine cpu is 32bit or 64bit");
            }

            SevenZipCompressor.SetLibraryPath(dll);
            SevenZipExtractor.SetLibraryPath(dll);
        }
    }
}