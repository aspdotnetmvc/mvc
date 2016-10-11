using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSModel
{
    [Serializable]
    public class Annunciate
    {
        public string AnnunciateAccount { get; set; }
        public string AnnunciateID { get; set; }
        public string AnnunciateTitle { get; set; }
        public string AnnunciateContent { get; set; }
        public DateTime CreateTime { get; set; }
        public AnnunciateType Type { get; set; }
        public List<String> Users { get; set; }
        public SysPlatType PlatType { get; set; }
    }
}
