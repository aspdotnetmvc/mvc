using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageTools
{
    public interface IMessageContainer
    {
        void WriteMessage(Message message);
    }
}
