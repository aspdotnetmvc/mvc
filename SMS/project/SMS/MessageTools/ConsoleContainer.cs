using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageTools
{
    public class ConsoleContainer : IMessageContainer
    {
        public void WriteMessage(Message message)
        {
            string info = message.Time + " " + message.Level.ToString() + " " + message.message;
            Console.WriteLine(info);
            if (message.Exception != null)
            {
                Console.WriteLine(message.Exception.ToString());
            }
        }
    }
}
