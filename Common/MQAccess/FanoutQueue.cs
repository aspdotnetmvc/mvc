using RabbitMQ.Client;
using RabbitMQ.Client.Content;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace MQAccess
{
    public class FanoutQueue : Fanout
    {
        Queue<string> _sends;
        object _lock = new object();
        Thread _sendThread;

        public FanoutQueue(string virtualHost, string url, string name, string password, string exchange, string[] queues)
            :base(virtualHost,url,name,password,exchange,queues)
        {
            _sends = new Queue<string>();
            _sendThread = new Thread(new ThreadStart(SendThread));
            _sendThread.Start();
        }

        public new void Send(string message)
        {
            bool sok = true;
            try
            {
                sok = base.Send(message);
            }
            catch
            {

            }

            if (!sok)
            {
                lock (_lock)
                {
                    _sends.Enqueue(message);
                }
            }

        }

        private void SendThread()
        {
            while (true)
            {
                if (base._ctsQuit.Token.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    while (_sends.Count > 0)
                    {
                        lock (_lock)
                        {

                            string s = _sends.Dequeue();
                            if (!base.Send(s))
                            {
                                _sends.Enqueue(s);
                                break;
                            }
                        }
                    }
                }
                catch
                {

                }
                Thread.Sleep(1000);
            }
        }
    }
}
