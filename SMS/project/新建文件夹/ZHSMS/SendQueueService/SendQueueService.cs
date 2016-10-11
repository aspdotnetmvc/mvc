using SendQueueInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendQueueService
{
    public class SendQueueService : MarshalByRefObject, IBlacklist
    {
        public void UpdateBlacklistCache(List<string> numbers)
        {
            BlacklistManager.Instance.Add(numbers);
        }
    }
}
