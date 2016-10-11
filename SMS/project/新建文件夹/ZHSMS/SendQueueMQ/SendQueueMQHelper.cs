using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace SendQueueMQ
{
    public class MQHelper
    {
        public static string UserName = "";
        public static string Password = "";
        public static string VirtualHost = "";
        public static string HostName = "";
        public IConnection CreateConnection()
        {
             ConnectionFactory factory = new ConnectionFactory();
            factory.AutomaticRecoveryEnabled = true;
            factory.UserName = UserName;
            factory.Password = Password;
            factory.VirtualHost = VirtualHost;
            factory.HostName = HostName;
            IConnection conn = factory.CreateConnection();
            return conn;
        }


        public void SendMQ(string message)
        {

        }
    }
}
