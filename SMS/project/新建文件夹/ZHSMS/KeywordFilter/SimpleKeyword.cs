using KeywordFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordFilter
{
    public class SimpleKeyword
    {
        Keyword _keyword;
        FindKeyword _kf;

        public SimpleKeyword(Keyword keyword)
        {
            _kf = new FindKeyword();
            _kf.Init(keyword.Words.ToArray());
            _keyword = keyword;
            _keyword.UpdateKeyword += _keyword_UpdateKeyword;
        }

        void _keyword_UpdateKeyword(object sender, EventArgs e)
        {
            _kf.Init(_keyword.Words.ToArray());
        }

        public string Replace(string text)
        {
            return _kf.Replace(text,'*');
        }

        public string[] Find(string text)
        {
            string[] rs;
            KeywordCache.Instance.Get(text,out rs);
            if (rs == null)
            {
                rs = _kf.Find(text);
                KeywordCache.Instance.Add(text, rs);
            }
            return rs;
        }
    }
}
