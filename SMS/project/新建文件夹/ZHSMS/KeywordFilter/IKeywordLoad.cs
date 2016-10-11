using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordFilter
{
    public interface IKeywordLoad
    {
        List<string> Load();
        event EventHandler UpdateKeyword;
    }
}
