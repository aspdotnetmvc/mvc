using BXM.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SMSUtils
{
    public class DebugLog:IDisposable
    {
        private volatile static DebugLog mng = null;
        private static object lockHelper = new object();


        string _fileName;
        FileStream _fileStream;

        private DebugLog()
        {
            InitWrite();
        }

        private string Get_YYYYMMDD_String(DateTime dt)
        {
            string s = dt.Year.ToString().PadLeft(4, '0');
            s += dt.Month.ToString().PadLeft(2, '0');
            s += dt.Day.ToString().PadLeft(2, '0');
            return s;
        }

        public static DebugLog Instance
        {
            get
            {
                if (mng == null)
                {
                    lock (lockHelper)
                    {
                        if (mng == null)
                        {
                            mng = new DebugLog();
                            return mng;
                        }
                    }
                }
                return mng;
            }
        }

        private void InitWrite()
        {
            if (_fileStream != null)
            {
                _fileStream.Flush();
                _fileStream.Close();
                _fileStream = null;
            }
            _fileName = "debug_" + Get_YYYYMMDD_String(DateTime.Now) + ".log";
            _fileStream = new FileStream(_fileName, FileMode.Append);
        }

        public void Write<T>(string describe, T obj)
        {
            string text = DateTime.Now.ToString() + " , " + describe + " , " + JsonSerialize.Instance.Serialize<T>(obj) + "\r\n\r\n";
            Console.WriteLine("LOG Write -> " + text);
            byte[] data = new UTF8Encoding().GetBytes(text);

            write:
            try
            {
                IAsyncResult async = _fileStream.BeginWrite(data, 0, data.Length, asyncResult =>
                {
                    _fileStream.EndWrite(asyncResult);

                }, null);
            }
            catch
            {
                InitWrite();
                Thread.Sleep(100);
                goto write;
            }
        }

        public void Write(string describe)
        {
            string text = DateTime.Now.ToString() + " , " + describe + "\r\n\r\n";
            Console.WriteLine("LOG Write -> " + text);
            byte[] data = new UTF8Encoding().GetBytes(text);

        write:
            try
            {
                IAsyncResult async = _fileStream.BeginWrite(data, 0, data.Length, asyncResult =>
                {
                    _fileStream.EndWrite(asyncResult);

                }, null);
            }
            catch
            {
                InitWrite();
                Thread.Sleep(100);
                goto write;
            }
        }

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Flush();
                _fileStream.Close();
            }
        }
    }
}
