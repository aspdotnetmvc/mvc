using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendQueueInterface
{
    public interface IBlacklist 
    {
        void UpdateBlacklistCache(List<string> numbers);
    }
}
