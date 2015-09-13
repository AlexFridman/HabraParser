using HabraMiner.Articles;
using MongoDB.Driver;
using NLog;

namespace HabraMiner.Storage
{
    public class MongoArticleSaver<TArticle> : IArticleSaver<TArticle> where TArticle: ArticleBase
    {
        private readonly MongoCollection<TArticle> _collection;
        private static Logger Logger = LogManager.GetLogger("Saver");

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
            Logger.Info($"Saved {article.Uri.AbsolutePath}");
        }
    }
}