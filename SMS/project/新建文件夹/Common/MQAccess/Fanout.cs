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
    public class Fanout:IDisposable
    {
        ConnectionFactory connectionFactory;
        IConnection _conn;
        IModel _channel;

        string _exchange;
        string[] _queues;
        Thread _reconnectionThread;
        protected CancellationTokenSource _ctsQuit = new CancellationTokenSource();

        public Fanout(string virtualHost, string url, string name, string password, string exchange, string[] queues)
        {
            _exchange = exchange;
            _queues = queues;

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
            catch(Exception ex)
            {
                throw ex;
            }

            _reconnectionThread = new Thread(new ThreadStart(ReconnectionThreadRun));
            _reconnectionThread.Start();
        }

        void ReconnectionThreadRun()
        {
            while (true)
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
                }
                Thread.Sleep(100);
            }
        }

        private void Iinitialize()
        {
            _conn = connectionFactory.CreateConnection();
            _channel = _conn.CreateModel();
            _channel.ExchangeDeclare(_exchange, ExchangeType.Fanout);
            foreach (var queue in _queues)
            {
                _channel.QueueDeclare(queue, true, false, false, null);
                _channel.QueueBind(queue, _exchange, "");
            }
        }

        public bool Send(string message)
        {
            if (!_channel.IsOpen) return false;
            try
            {
                _channel.BasicPublish(_exchange, "", null, System.Text.Encoding.UTF8.GetBytes(message));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 返回队列未处理信息数量
        /// </summary>
        /// <param name="queueName">队列名称</param>
        /// <returns>未处理数据数量，-1为获取错误</returns>
        public int GetQueueCount(string queueName)
        {
            if (!_channel.IsOpen) return -1;
            try
            {
                BasicGetResult r = _channel.BasicGet(queueName, false);
                if (r == null) return 0;
                _channel.BasicReject(r.DeliveryTag, true);
                return (int)r.MessageCount;
            }
            catch
            {
                return -1;
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
