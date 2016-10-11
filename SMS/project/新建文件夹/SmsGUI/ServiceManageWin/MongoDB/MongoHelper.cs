using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB
{
    public class MongoHelper
    {
        #region 创建数据库，获取集合
        protected static IMongoDatabase CreateDatabase()
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["mongodb"];
            string databaseName = System.Configuration.ConfigurationManager.AppSettings["database"];
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            return database;
        }
        /// <summary>
        /// 获取操作对象的IMongoCollection集合,强类型对象集合
        /// </summary>
        /// <returns></returns>
        public static IMongoCollection<T> GetCollection<T>(string entitysName) where T : MongoModel
        {
            var database = CreateDatabase();
            return database.GetCollection<T>(entitysName);
        }
        #endregion
    }
}
