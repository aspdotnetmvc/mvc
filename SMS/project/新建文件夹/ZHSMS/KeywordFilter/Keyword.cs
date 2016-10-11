using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordFilter
{
    public class Keyword
    {
        List<string> _words;
        IKeywordLoad _update;
        public event EventHandler UpdateKeyword;

        public Keyword(IKeywordLoad keywordUpdate)
        {
            _update = keywordUpdate;
            _update.UpdateKeyword += _update_UpdateKeyword;

            _words = _update.Load();
        }

        void _update_UpdateKeyword(object sender, EventArgs e)
        {
            if (UpdateKeyword != null)
            {
                UpdateKeyword(this, e);
            }
        }

        public List<string> Words
        {
            get { return _words; }
        }
    }
}
