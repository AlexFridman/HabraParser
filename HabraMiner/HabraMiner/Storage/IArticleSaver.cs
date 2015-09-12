namespace HabraMiner.Storage
{
    public interface IArticleSaver<in TArticle>
    {
        void Save(TArticle article);
    }
}
