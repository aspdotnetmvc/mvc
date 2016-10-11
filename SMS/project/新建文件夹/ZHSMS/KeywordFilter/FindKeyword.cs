using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeywordFilter
{
    public class FindKeyword
    {
        Dictionary<char, List<string>> _keywords;

        public FindKeyword()
        {
            _keywords = new Dictionary<char, List<string>>();
        }

        public static int FastIndexOf(string source, string pattern)
        {
            bool found;
            int limit = source.Length - pattern.Length + 1;
            if (limit < 1) return -1;

            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern.Length > 1 ? pattern[1] : ' ';

            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, 0, limit);

            while (first != -1)
            {
                // Check if the following character is the same like the 2nd character of "pattern"
                if (pattern.Length > 1 && source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }

                // Check the rest of "pattern" (starting with the 3rd character)
                found = true;
                for (int j = 2; j < pattern.Length; j++)
                    if (source[first + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }

                // If the whole word was found, return its index, otherwise try again
                if (found) return first;
                first = source.IndexOf(c0, ++first, limit - first);
            }
            return -1;
        }

        public void Init(string[] keywords)
        {
            _keywords.Clear();
            
            foreach(string keyword in keywords)
            {
                List<string> klist;
                _keywords.TryGetValue(keyword[0], out klist);
                if (klist != null)
                {
                    klist.Add(keyword);
                }
                else
                {
                    klist = new List<string>();
                    klist.Add(keyword);
                    _keywords.Add(keyword[0], klist);
                }
            }
        }

        unsafe public Dictionary<int, int> FindIndexOf(string text)
        {
            var findResult = new Dictionary<int, int>();

            if (_keywords == null || _keywords.Count == 0) return findResult;

            List<string> klist;
            fixed (char* source = text)
            {
                int tl = text.Length;
                for (int i = 0; i < tl; i++)
                {
                    _keywords.TryGetValue(source[i], out klist);
                    if (klist != null)
                    {
                        foreach (string k in klist)
                        {
                            fixed (char* key = k)
                            {
                                int kl = k.Length;
                                if (kl > tl - i)
                                {
                                    continue;
                                }
                                bool find = true;
                                for (int j = kl - 1; j > 0; j--)
                                {
                                    if (key[j] == source[i + j])
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        find = false;
                                        break;
                                    }
                                }
                                if (find)
                                {
                                    findResult.Add(i, kl);
                                    i += kl - 1;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return findResult;
        }

        unsafe public string Replace(string source, char replace)
        {
            var dic = FindIndexOf(source);
            if (dic.Count == 0) return source;

            fixed (char* newText = source)
            {
                var c = newText;
                foreach (var i in dic)
                {
                    c = newText + i.Key;
                    for (var index = 0; index < i.Value; index++)
                    {
                        *c++ = replace;
                    }
                }
            }
            return source;
        }

        public string[] Find(string source)
        {
            var dic = FindIndexOf(source);
            string[] rs = new string[dic.Count];
            if (dic.Count == 0) return rs;

            int i=0;
            foreach(int k in dic.Keys)
            {
                rs[i] = source.Substring(k, dic[k]);
                i++;
            }
            return rs;
        }

    }
}
