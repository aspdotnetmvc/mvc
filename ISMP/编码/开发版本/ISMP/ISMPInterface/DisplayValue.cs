using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISMPInterface
{
    [Serializable]
    public class DisplayValue<D,V>
    {
        public D Display { get; set; }
        public V Value { get; set; }

        public DisplayValue()
        {

        }
        public DisplayValue(D display,V value)
        {
            Display = display;
            Value = value;
        }

        public override string ToString()
        {
            return Display.ToString();
        }
    }
}
