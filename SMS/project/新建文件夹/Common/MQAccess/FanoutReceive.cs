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
    public class FanoutReceive:IDisposable
    {
        ConnectionFactory connectionFactory;
        IConnection _conn;
        IModel _channel;

        bool _ack;
        string _queue;
        //100 second 重收 
        const ushort _receiveTime = 1000;//1000; //100秒
        Thread _reconnectionThread;
        Thread _receiveThread;
        ushort _qosSize = 3;

        CancellationTokenSource _ctsQuit = new CancellationTokenSource();

        int _recover = 0;

        public delegate bool ReceiveMessageHandler(string queue,string message);
        public event ReceiveMessageHandler ReceiveMessage;

        public FanoutReceive(string virtualHost, string url, string name, string password, string queue, bool ack, ushort qosSize)
        {
            _queue = queue;
            _ack = ack;
            _qosSize = qosSize;

            connectionFactory = new ConnectionFactory();
            connectionFactory.UserName = name;
            connectionFactory.Password = password;
            connectionFactory.VirtualHost = virtualHost;
            connectionFactory.RequestedHeartbeat = 0;
            connectionFactory.Endpoint = new AmqpTcpEndpoint(new Uri(url));

            try
            {
                Iinitialize();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            _receiveThread = new Thread(new ThreadStart(SubscriptionMessage));
            _receiveThread.Start();

            _reconnectionThread = new Thread(new ThreadStart(ReconnectionThreadRun));
            _reconnectionThread.Start();
        }


        void ReconnectionThreadRun()
        {
            while (true)
            {
                try
                {
                    if (_ctsQuit.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (!_channel.IsOpen)
                    {
                        try
                        {
                            Iinitialize();
                        }
                        catch
                        {
                            Thread.Sleep(5000);
                        }
                        continue;
                    }

                    if (_recover++ >= _receiveTime)
                    {
                        _recover = 0;
                        try
                        {
                            //_channel.BasicRecover(true);
                        }
                        catch
                        {
                        }
                    }
                    Thread.Sleep(100);
                }
                catch
                {
                }
            }
            
        }

        private void Iinitialize()
        {
            _conn = connectionFactory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.QueueDeclare(_queue, true, false, false, null);
        }

        private void SubscriptionMessage()
        {
            while (true)
            {
                if (_ctsQuit.Token.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    if (_channel.IsOpen)
                    {
                        _channel.BasicQos(0, _qosSize, false);
                        QueueingBasicConsumer consumer = new QueueingBasicConsumer(_channel);
                        _channel.BasicConsume(_queue, !_ack, consumer);

                        while (_channel.IsOpen)
                        {
                            try
                            {
                                BasicDeliverEventArgs e = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                                if (e != null)
                                {
                                    byte[] body = e.Body;
                                    if (ReceiveMessage != null)
                                    {
                                        try
                                        {
                                            bool processed = ReceiveMessage(_queue, System.Text.Encoding.UTF8.GetString(body));
                                            if (_ack)
                                            {
                                                if (processed)
                                                    _channel.BasicAck(e.DeliveryTag, false);
                                                else
                                                    _channel.BasicReject(e.DeliveryTag, true);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _channel.BasicReject(e.DeliveryTag, true);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                     
                }
                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            _ctsQuit.Cancel();
            _channel.Close();
            _conn.Close();
        }
    }
}
