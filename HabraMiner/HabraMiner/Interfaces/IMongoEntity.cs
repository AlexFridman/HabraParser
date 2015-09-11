using MongoDB.Bson;
using MongoDB.Shared;

namespace HabraMiner.Interfaces
{
    public interface IMongoEntity
    {
        BsonDocument InternalDocument { get; set; }
    }
}