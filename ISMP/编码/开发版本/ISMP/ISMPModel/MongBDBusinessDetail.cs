using ISMPInterface;
using ISMPModel;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountCenter
{
    /// <summary>
    /// 业务明细
    /// </summary>
    [Serializable]
    public class MBDBusinessDetail : BusinessDetail
    {
        public ObjectId _id;//BsonType.ObjectId 这个对应了 MongoDB.Bson.ObjectId 
    }
}
