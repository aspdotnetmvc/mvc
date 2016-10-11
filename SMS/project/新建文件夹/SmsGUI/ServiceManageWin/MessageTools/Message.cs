using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageTools
{
    [Serializable]
    public class Message
    {
        public MessageLevel Level { get; set; }
        public string message { get; set; }
        public Exception Exception { get; set; }
        public DateTime Time { get; set; }
    }
}
