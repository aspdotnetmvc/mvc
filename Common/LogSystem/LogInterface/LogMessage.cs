using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogInterface
{
    [Serializable]
    public class LogMessage
    {
        public string Level { get; set; }
        public string Thread {get;set;}
        public string Ip{get;set;}
        public string Recorder{get;set;}
        public string Event{get;set;}
        public string Message{get;set;}
        public DateTime Date { get; set; } 
    }
}
