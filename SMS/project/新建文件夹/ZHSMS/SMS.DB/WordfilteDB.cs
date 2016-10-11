using SMS.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SMS.DB
{
    public class WordfilteDB
    {
        /// <summary>
        /// 是否存在敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public static bool ExistGroup(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from KeywordsGroup ");
            strSql.Append(" where GroupName=@GroupName");
            var r = DBHelper.Instance.ExecuteScalar<int>(strSql.ToString(), new { GroupName = keyGroup });

            return r > 0;
        }
        /// <summary>
        /// 是否存在敏感词类别
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ExistType(string type)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from KeywordsType ");
            strSql.Append(" where KeywordsType=@KeywordsType");
            var r = DBHelper.Instance.ExecuteScalar<int>(strSql.ToString(), new { KeyWordsType = type });
            return r > 0;
        }
        /// <summary>
        /// 添加敏感词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static bool AddKeyGroup(string keyGroup, string remark)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into KeywordsGroup(");
            strSql.Append("GroupName,Remark)");
            strSql.Append(" values (");
            strSql.Append("@GroupName,@Remark)");

            DBHelper.Instance.Execute(strSql.ToString(), new { GroupName = keyGroup, Remark = remark });
            return true;


        }
        /// <summary>
        /// 添加敏感词类别
        /// </summary>
        /// <param name="type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static bool AddKeyType(string type, string remark)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into KeywordsType(");
            strSql.Append("KeywordsType,Remark)");
            strSql.Append(" values (");
            strSql.Append("@KeywordsType,@Remark)");
            DBHelper.Instance.Execute(strSql.ToString(), new { KeywordsType = type, Remark = remark });
            return true;
        }

        /// <summary>
        /// 添加敏感词语关联敏感词类别
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keyTypes"></param>
        /// <returns></returns>
        public static bool AddGroupTypeBind(string keyGroup, List<string> keyTypes)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into KeywordsGroupTypeBind(");
            strSql.Append("KeyGroup,KeyType)");
            strSql.Append(" values (");
            strSql.Append("@KeyGroup,@KeyType)");

            var entities = from kt in keyTypes select new { KeyGroup = keyGroup, KeyType = kt };
            DBHelper.Instance.Execute(strSql.ToString(), entities);
            return true;
        }
        /// <summary>
        /// 根据词组删除绑定的敏感词类型
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public static bool DelKeyTypesByGroup(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from KeywordsGroupTypeBind ");
            strSql.Append(" where KeyGroup=@KeyGroup");
            DBHelper.Instance.Execute(strSql.ToString(), new { KeyGroup = keyGroup });
            return true;
        }
        /// <summary>
        /// 删除词组
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public static bool DelKeyGroup(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from KeywordsGroup ");
            strSql.Append(" where GroupName=@GroupName");
            DBHelper.Instance.Execute(strSql.ToString(), new { GroupName = keyGroup });

            return true;
        }

        /// <summary>
        /// 添加敏感词
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static bool Add(string keyGroup, List<Keywords> keywords)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" insert into Wordfilte(");
            strSql.Append(" KeyGroup,Keywords,KeywordsType,Enabled,ReplaceKeywords)");
            strSql.Append(" select @KeyGroup,@Keywords,@KeywordsType,@Enabled,@ReplaceKeywords from dual ");
            strSql.Append(" where not exists (");
            strSql.Append(" select KeyGroup,Keywords from Wordfilte ");
            strSql.Append(" where KeyGroup=@KeyGroup and Keywords=@Keywords)");

            var kwords = from k in keywords select new { KeyGroup = keyGroup, Keywords = k.Words, KeywordsType = k.KeywordsType, Enabled = k.Enable, ReplaceKeywords = k.ReplaceKeywords };
            DBHelper.Instance.Execute(strSql.ToString(), kwords);
            return true;
        }
        /// <summary>
        /// 删除敏感词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static bool Del(string keyGroup, List<string> keywords)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from Wordfilte ");
            strSql.Append(" where KeyGroup=@KeyGroup and Keywords=@Keywords");
            var r = from k in keywords select new { KeyGroup = keyGroup, Keywords = k };
            DBHelper.Instance.Execute(strSql.ToString(), r);
            return true;
        }
        /// <summary>
        /// 根据词组获取敏感词
        /// </summary>
        public static List<Keywords> Gets(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeyGroup,Keywords as Words,KeywordsType,Enabled as Enable,ReplaceKeywords from Wordfilte ");
            strSql.Append(" where KeyGroup=@KeyGroup");
            return DBHelper.Instance.Query<Keywords>(strSql.ToString(), new { KeyGroup = keyGroup });
        }
        /// <summary>
        /// 根据类型获取敏感词
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Keywords> GetKeywordsByType(string type)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeyGroup,Keywords as Words,KeywordsType,Enabled as Enable,ReplaceKeywords from Wordfilte ");
            strSql.Append(" where KeywordsType=@KeywordsType");
            return DBHelper.Instance.Query<Keywords>(strSql.ToString(), new { KeywordsType = type });
        }
        /// <summary>
        /// 关键词是否启用
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <param name="keywords"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static bool KeywordsEnabled(string keyGroup, string keywords, bool enabled)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Wordfilte set ");
            strSql.Append("Enabled=@Enabled");
            strSql.Append(" where KeyGroup=@KeyGroup and Keywords=@Keywords");
            DBHelper.Instance.Execute(strSql.ToString(), new { Enabled = enabled == true ? 1 : 0, KeyGroup = keyGroup, Keywords = keywords });
            return true;
        }

        /// <summary>
        /// 更新敏感词（更新类型和替换其他词）
        /// </summary>
        public static bool Update(Keywords keyword)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update Wordfilte set ");
            strSql.Append("KeywordsType=@KeywordsType,");
            strSql.Append("ReplaceKeywords=@ReplaceKeywords");
            strSql.Append(" where KeyGroup=@KeyGroup and Keywords=@Words");
            DBHelper.Instance.Execute(strSql.ToString(), keyword);
            return true;
        }

        /// <summary>
        /// 获取词组的敏感词个数
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public static int GetCountKeywords(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(Keywords) from Wordfilte ");
            strSql.Append(" where KeyGroup=@KeyGroup");
            return DBHelper.Instance.ExecuteScalar<int>(strSql.ToString(), new { KeyGroup = keyGroup });

        }
        /// <summary>
        /// 根据词组获取启用的敏感词
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public static string[] Get(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Keywords from Wordfilte ");
            strSql.Append(" where KeyGroup=@KeyGroup and Enabled=1");

            return DBHelper.Instance.Query<string>(strSql.ToString(), new { KeyGroup = keyGroup }).ToArray();

        }

        /// <summary>
        /// 获取敏感词组
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetKeyGroups()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from KeywordsGroup ");

            var kg = DBHelper.Instance.Query(strSql.ToString());

            var r = from k in kg select new { Key = k.GroupName, Value = k.Remark };
            return r.ToDictionary(k => Convert.ToString((object)k.Key), k => Convert.ToString((object)k.Value));

        }

        /// <summary>
        /// 获取敏感词类别
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetKeyTypes()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from KeywordsType ");
            var kg = DBHelper.Instance.Query(strSql.ToString());

            var r = from k in kg select new { Key = k.KeywordsType, Value = k.Remark };
            return r.ToDictionary(k => Convert.ToString((object)k.Key), k => Convert.ToString((object)k.Value));

        }
        /// <summary>
        /// 根据词组获取敏感词类别
        /// </summary>
        /// <param name="keyGroup"></param>
        /// <returns></returns>
        public static List<string> GetKeyTypesByGroup(string keyGroup)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeyType from KeywordsGroupTypeBind ");
            strSql.Append(" where KeyGroup=@KeyGroup");
            return DBHelper.Instance.Query<string>(strSql.ToString(), new { KeyGroup = keyGroup });
        }

        /// <summary>
        /// 根据敏感词类型获取敏感词组
        /// </summary>
        /// <param name="keyType"></param>
        /// <returns></returns>
        public static List<string> GetKeywordsGroupByType(string keyType)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeyGroup from KeywordsGroupTypeBind ");
            strSql.Append(" where KeyType=@KeyType");
            return DBHelper.Instance.Query<string>(strSql.ToString(), new { KeyType = keyType });


        }
        /// <summary>
        /// 根据词获取敏感词（模糊查询）
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<Keywords> GetKeywordsByKeyword(string keyword)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeywordsType from Wordfilte ");
            strSql.Append(" where Keywords like @Keywords");

            return DBHelper.Instance.Query<Keywords>(strSql.ToString(), new { Keywords = "%" + keyword + "%" });
        }

        public static int GetAllKeywordCount()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from Wordfilte ");
            return DBHelper.Instance.ExecuteScalar<int>(strSql.ToString());
        }

        public static List<Keywords> GetAllKeywords(int pageIndex, int pageSize)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select KeyGroup,Keywords as Words,KeywordsType,Enabled as Enable,ReplaceKeywords  from Wordfilte ");
            strSql.Append(" limit " + pageIndex * pageSize + "," + pageSize + "");
            return DBHelper.Instance.Query<Keywords>(strSql.ToString());

        }

    }
}
