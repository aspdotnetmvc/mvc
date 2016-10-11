using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordFilter
{
    public class ComplexKeyword
    {
        Keyword _keyword;


        public ComplexKeyword(Keyword keyword)
        {
            FilterKeywordsFast.Init(keyword.Words);
            _keyword = keyword;
            _keyword.UpdateKeyword += _keyword_UpdateKeyword;
        }

        void _keyword_UpdateKeyword(object sender, EventArgs e)
        {
            FilterKeywordsFast.Init(_keyword.Words);
        }

        public string Replace(string text)
        {
            //FilterKeyWordsFast.MapChar(text);
            return FilterKeywordsFast.Replace(text);
        }

        public string[] Find(string text)
        {
            //FilterKeyWordsFast.MapChar(text);
            string[] hasKeys;
            FilterKeywordsFast.Find(text, out hasKeys);
            return hasKeys;
        }
    }
}
