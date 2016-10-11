using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDBAccess
{
    public class MongoDBHelper
    {
        private volatile static MongoDBHelper _instance = null;
        private static readonly object lockHelper = new object();
        MongoClient _client;
        MongoServer _server;
        MongoDatabase _db;

        private MongoDBHelper()
        {
            try
            {
                string connection = System.Configuration.ConfigurationManager.AppSettings["MongoDBUrl"];
                string dbName = System.Configuration.ConfigurationManager.AppSettings["MongoDBDatabase"];
                _client = new MongoClient(connection);
                _server = _client.GetServer();
                _db = _server.GetDatabase(dbName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static MongoDBHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockHelper)
                    {
                        if (_instance == null)
                            _instance = new MongoDBHelper();
                    }
                }
                return _instance;
            }
        }

        public List<T> GetCollection<T>(string collectionName)
        {
            MongoCollection cols = _db.GetCollection(collectionName);
            return cols.FindAllAs<T>().ToList();
        }

        public List<T> GetCollection<T>(string collectionName, int skip, int take)
        {
            MongoCollection cols = _db.GetCollection(collectionName);
            return cols.FindAllAs<T>()
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        public List<T> GetCollection<T>(string collectionName, IMongoQuery query, int skip, int take)
        {
            MongoCollection cols = _db.GetCollection(collectionName);
            return cols.FindAs<T>(query)
                .Skip(skip)
                .Take(take)
                .ToList();
        }

        public List<T> GetCollection<T>(string collectionName, IMongoQuery query)
        {
            MongoCollection cols = _db.GetCollection(collectionName);
            return cols.FindAs<T>(query).ToList();
        }

        public T GetCollectionByOne<T>(string collectionName, IMongoQuery query)
        {
            MongoCollection cols = _db.GetCollection(collectionName);
            return cols.FindOneAs<T>(query);
        }

        public void Insert<T>(string collectionName, T content)
        {
            MongoCollection cols = _db.GetCollection(collectionName);
            cols.Insert<T>(content);
        }

        public void InsertArray<T>(string collectionName, IEnumerable<T> content)
        {
            BsonArray array = new BsonArray(content);
            MongoCollection cols = _db.GetCollection(collectionName);
            cols.Insert(array);
        }
    }
}
