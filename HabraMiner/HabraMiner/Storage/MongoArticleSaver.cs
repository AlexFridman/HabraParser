using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HabraMiner.Articles;
using MongoDB.Driver;
using NLog;

namespace HabraMiner.Storage
{
    public class MongoArticleSaver<TArticle> : IArticleSaver<TArticle> where TArticle: ArticleBase
    {
        private readonly MongoCollection<TArticle> _collection;
        private static Logger Logger = LogManager.GetLogger("Saver");
        private readonly ConcurrentQueue<TArticle> _articlesToSaveQueue = new ConcurrentQueue<TArticle>();
        private readonly int _batchCount;

        public MongoArticleSaver(string adress, int port, string databaseName, string collectionName, int batchCount = 50)
        {
            var settings = new MongoServerSettings {Server = new MongoServerAddress(adress, port)};
            var server = new MongoServer(settings);
            var database = server.GetDatabase(databaseName);
            _collection = database.GetCollection<TArticle>(collectionName);
            _batchCount = batchCount;
        }
        public void Save(TArticle article)
        {
            _articlesToSaveQueue.Enqueue(article);
            if (_articlesToSaveQueue.Count > _batchCount)
            {
                lock (_articlesToSaveQueue)
                {
                    var articlesToSave = GetBatch(_articlesToSaveQueue, _batchCount);
                    Task.Run(() => SaveBatch(articlesToSave));
                }
            }
            //Logger.Info($"Added to save queue {article.Uri.AbsolutePath}");
        }

        private ICollection<TArticle> GetBatch(ConcurrentQueue<TArticle> articles, int count)
        {
            var result = new List<TArticle>();
            var i = 0;
            while (i < count && !articles.IsEmpty)
            {
                TArticle article;
                articles.TryDequeue(out article);
                result.Add(article);

                i++;
            }
            return result;
        }

        public void SaveBatch(ICollection<TArticle> articles)
        {
            _collection.InsertBatch(articles);
            Logger.Info($"Batch saved ({_batchCount}). First {articles.First().Uri.AbsolutePath}");
        }

        public void Flush()
        {
            SaveBatch(_articlesToSaveQueue.ToList());
        }

       
    }
}