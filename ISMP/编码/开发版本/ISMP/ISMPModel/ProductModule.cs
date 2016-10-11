using ISMPInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPModel
{
     [Serializable]
    public class ProductModule : ProductFunction, IProductModule
    {
        public List<IPermission> Permissions { get; set; }
        public  int OrderNumber { get; set; }
    }
}
