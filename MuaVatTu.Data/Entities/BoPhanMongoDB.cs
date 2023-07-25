using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace MuaVatTu.Data
{
    [Serializable, BsonIgnoreExtraElements]
    public class BoPhanMongoDB
    {
        [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonElement("nameOfLeader"), BsonRepresentation(BsonType.String)]
        public string NameOfLeader { get; set; }
        [BsonElement("date"), BsonRepresentation(BsonType.String)]
        public string Date { get; set; }
        [BsonElement("number"), BsonRepresentation(BsonType.Int32)]
        public int Number { get; set; }
    }
}
