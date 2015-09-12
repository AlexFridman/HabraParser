using HabraMiner.Articles;
using MongoDB.Driver;

namespace HabraMiner
{
    public class MongoArticleSaver<TArticle> : IArticleSaver<TArticle>
    {
        private readonly MongoCollection<TArticle> _collection;


        public MongoArticleSaver(string adress, int port, string databaseName, string collectionName)
        {
            var settings = new MongoServerSettings {Server = new MongoServerAddress(adress, port)};
            var server = new MongoServer(settings);
            var database = server.GetDatabase(databaseName);
            _collection = database.GetCollection<TArticle>(collectionName);
        }
        public void Save(TArticle article)
        {
            _collection.Save(article);
        }
    }
}