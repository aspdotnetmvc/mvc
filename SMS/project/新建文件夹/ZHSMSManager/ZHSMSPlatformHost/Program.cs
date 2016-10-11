using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;

namespace ZHSMSPlatformHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Starting the server!");

            RemotingConfiguration.Configure("ZHSMSPlatformHost.exe.config", false);
            Console.WriteLine(DateTime.Now.ToString() + " Started the server ok.");

            Console.WriteLine();
            Console.WriteLine("Press input 'quit' to stop it!");

            do
            {
            } while (Console.ReadLine() != "quit");
            Console.WriteLine("The server was stopped!");
        }
    }
}
