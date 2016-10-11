using SMS.Model;
using System;

namespace GatewayInterface
{
    [Serializable]
    public class DeliverEventArgs : EventArgs
    {
        public MOSMS MO
        {
            get;
            set;
        }

        public DeliverEventArgs(MOSMS mo)
        {
            this.MO = mo;
        }
        
    }
}
