using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace MQHelper
{
    public class RabbitMQHelper
    {
        #region 属性
        private string Host { get; set; }
        private string VHost { get; set; }
        private string Exchange { get; set; }

        private string Queue { get; set; }
        private string UserName { get; set; }

        private string Password { get; set; }

        public IConnection Connection
        {
            get;
            set;
        }
        public IModel Channel
        {
            get;
            set;
        }
        #endregion

        #region
        public List<string> AckfailureMessage = new List<string>();
        #endregion

        public RabbitMQHelper(string host, string vhost, string username, string password)
        {
            this.Host = host;
            this.VHost = vhost;
            this.UserName = username;
            this.Password = password;
            ConnectionFactory factory = new ConnectionFactory();
            factory.AutomaticRecoveryEnabled = true;
            factory.HostName = Host;
            factory.UserName = UserName;
            factory.VirtualHost = VHost;
            factory.Password = Password;
            factory.RequestedHeartbeat = 5;


            Connection = factory.CreateConnection();

            Channel = Connection.CreateModel();
        }

        #region 发布消息

        /// <summary>
        /// 绑定网关和队列可以绑定多个队列
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queues"></param>
        public void BindQueue(string exchange, int maxpriority, params string[] queues)
        {
            this.Exchange = exchange;

            Channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
            IDictionary<string, object> args = new Dictionary<string, object>();
            args.Add("x-max-priority", maxpriority);
            foreach (var queue in queues)
            {
                Channel.QueueDeclare(queue, true, false, false, args);
                Channel.QueueBind(queue, Exchange, Exchange);
            }

        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool PublishMessage(string message, int priorty = 0)
        {
            try
            {
                if (Channel.IsOpen)
                {
                    IBasicProperties bp = Channel.CreateBasicProperties();
                    bp.Priority = (byte)priorty;
                    bp.Persistent = true;
                    Channel.BasicPublish(Exchange, Exchange, bp, Encoding.UTF8.GetBytes(message));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageTools.MessageHelper.Instance.WirteError("MQ:发布消息时发生了异常", ex);
                return false;
            }
        }

        #endregion

        #region 订阅消息
        /// <summary>
        /// 开始订阅消息
        /// </summary>
        public void SubsribeMessage(string queue, int maxpriority=10, bool noAck = false)
        {
            this.Queue = queue;
            IDictionary<string, object> args = new Dictionary<string, object>();
            args.Add("x-max-priority", maxpriority);
            Channel.QueueDeclare(queue, true, false, false, args);
            Channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(this.Channel);
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body);

                    if (OnSubsribeMessageRecieve != null)
                    {

                        var succ = false;
                        if (ea.Redelivered)
                        {
                            if (AckfailureMessage.Contains(body))
                            {
                                Channel.BasicAck(ea.DeliveryTag, false);
                                AckfailureMessage.Remove(body);
                                return;
                            }
                            if (OnSubsribeMessageRecieve_Redelivered != null)
                            {

                                succ = OnSubsribeMessageRecieve_Redelivered(this, body);

                            }
                            else
                            {

                                succ = OnSubsribeMessageRecieve(this, body);
                            }
                        }
                        else
                        {
                            succ = OnSubsribeMessageRecieve(this, body);
                        }

                        if (noAck)
                        {
                            return;
                        }
                        if (succ)
                        {

                            if (Connection.IsOpen)
                            {
                                Channel.BasicAck(ea.DeliveryTag, false);
                            }
                            else
                            {
                                //消息处理成功，ack失败时，记录消息
                                AckfailureMessage.Add(body);
                            }

                        }
                        else
                        {
                            Channel.BasicReject(ea.DeliveryTag, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //类库中记录日志
                    MessageTools.MessageHelper.Instance.WirteError("MQ:处理消息时发生了异常", ex);
                }
            };
            String consumerTag = this.Channel.BasicConsume(this.Queue, noAck, consumer);

        }


        public delegate bool OnRecieveMessage(RabbitMQHelper mq, string message);

        public OnRecieveMessage OnSubsribeMessageRecieve = null;
        /// <summary>
        /// 重复获取的消息，应检测上次执行结果，判断是否重新执行
        /// </summary>
        public OnRecieveMessage OnSubsribeMessageRecieve_Redelivered = null;
        #endregion

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Close()
        {
            try
            {
                this.Channel.Close(200, "Goodbye");
                this.Connection.Close();
            }
            catch (Exception ex) { }
        }
    }
}
