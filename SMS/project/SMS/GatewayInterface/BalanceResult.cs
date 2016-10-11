using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayInterface
{
    [Serializable]
    public class BalanceResult
    {
        public bool Success { get; set; }
        public int Balance { get; set; }
        public string Message { get; set; }
    }
}
