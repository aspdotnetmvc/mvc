using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZabbixAgentExtend
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2) return;
            string server = args[0];
            string function = args[1];
            switch (function)
            {
                case "ALIVE":
                    Console.Write(MonitoringProxy.Instance.Heartbeat(server).ToString().ToLower());
                    break;
            }
        }
    }
}
