using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    [Serializable]
    public class StatusResult
    {
       public bool Success { get; set; }
       public string SerialNumber { get; set; }

       public string Number { get; set; }
       public string Message { get; set; }
       public int StatusCode { get; set; }
    }
}
