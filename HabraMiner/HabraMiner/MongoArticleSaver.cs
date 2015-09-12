using HabraMiner.Articles;
using MongoDB.Driver;

namespace HabraMiner
{
    public class MongoArticleSaver<TArticle> : IArticleSaver<TArticle>
    {
        private readonly IMongoCollection<TArticle> _collection;

        public MongoArticleSaver(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<TArticle>(collectionName);
        }
        public void Save(TArticle article)
        {
            _collection.InsertOneAsync(article);
        }
    }
}