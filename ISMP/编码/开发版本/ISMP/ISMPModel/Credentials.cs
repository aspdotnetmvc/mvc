using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
    [Serializable]
    public class Credentials
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string CredentialsType { get; set; }
        public string Note { get; set; }
        public string FilePath { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductId { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
    }
}
