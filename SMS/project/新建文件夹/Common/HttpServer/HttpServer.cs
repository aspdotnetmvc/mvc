using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BXM.HttpServer
{
    public delegate void ErrorHandler(Exception error);
    public delegate void LogHandler(string log);

    public abstract class HttpServer
    {
        protected int port;
        TcpListener listener;
        bool is_active = true;
        public event LogHandler Log;
        public event ErrorHandler Error;

        public bool IsActive
        {
            get { return is_active; }
        }

        Thread httpThread;
        public HttpServer(int port)
        {
            this.port = port;
        }

        private void listen()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
            }
            catch (Exception ex)
            {
                processor_Error(ex);
                return;
            }

            while (is_active)
            {
                TcpClient s = listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                processor.Error += processor_Error;
                processor.Log += processor_Log;
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        void processor_Log(string log)
        {
            if(this.Log!=null)
            {
                Log(log);
            }
        }

        void processor_Error(Exception error)
        {
            if(this.Error!=null)
            {
                Error(error);
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);

        public void Start()
        {
            is_active = true;
            httpThread = new Thread(new ThreadStart(listen));
            httpThread.Start();
        }

        public void Stop()
        {
            listener.Stop();
            httpThread.Abort();
            is_active = false;
        }

        ~HttpServer()
        {
            Stop();
        }
    }
}
