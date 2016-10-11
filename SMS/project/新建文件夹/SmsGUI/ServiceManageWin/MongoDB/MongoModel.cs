using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDB
{
    [Serializable]
    public class MongoModel
    {
        /// <summary>
        /// Mongo ID
        /// </summary>
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public ObjectId _id { get; set; }
    }
}
