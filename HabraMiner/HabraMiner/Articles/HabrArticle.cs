using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using MongoDB.Bson;

namespace HabraMiner.Articles
{
    public class HabrArticle : ArticleBase
    {
        public ObjectId Id { get; set; }
        public string Author { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public ICollection<string> Habs { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; }

        public ICollection<string> CodeComments { get; set; }
        public ICollection<string> Tags { get; set; }
        public ICollection<string> Hubs { get; set; }
        public ICollection<string> Comments { get; set; }
        public int Number { get; set; }
        public int Rating { get; set; }
        public int Views { get; set; }
        public int Favourites { get; set; }

        public override ArticleBase Parse(string data)
        {
            return HabrParser.Parse(data);
        }

        public class HabrParser
        {
            public static HabrArticle Parse(string data)
            {
                var html = LoadHtmlDocument(data);

                var article = new HabrArticle();

                var articleNode = GetArticleNode(html);
                article.Name = ExtractName(articleNode);
                article.Date = ExtractDate(articleNode); //TODO : обойти  nginx
                article.Hubs = ExtractHabs(articleNode);
                article.Tags = ExtractTags(articleNode);
                article.Rating = ExtractRating(articleNode);
                article.Views = ExtractViews(articleNode);
                article.Favourites = ExtractFavourites(articleNode);
                article.CodeComments = ExtractCodeComments(articleNode);
                article.Author = ExtractAuthor(articleNode);

                DeleteImageNodes(articleNode);

                //DeleteCodeNodes(articleNode);TODO : обойти  nginx
                article.Text = articleNode.GetElementByClassName("content html_format").InnerText;

                article.Comments = ExtractComments(articleNode);

                return article;
            }

            private static void DeleteCodeNodes(HtmlNode articleNode)
            {
                var codeNotes = articleNode.GetElementsByClassName("nginx");
                foreach (var node in codeNotes)
                {
                    node.Remove();
                }
                codeNotes = articleNode.GetElementsByClassName("bash");
                foreach (var node in codeNotes)
                {
                    node.Remove();
                }
            }

            private static void DeleteImageNodes(HtmlNode articleNode)
            {
                var imageNotes = articleNode.GetElementsByTagName("img");
                foreach (var image in imageNotes)
                {
                    image.Remove();
                }
            }

            private static string ExtractAuthor(HtmlNode articleNode)
            {
                return HtmlHelpers.ReplaceHtml(articleNode.GetElementByClassName("author-info__name").InnerText);
            }

            private static List<string> ExtractCodeComments(HtmlNode articleNode)
            {
                var nginx = articleNode.GetElementsByClassName("nginx");
                return nginx?.Select(n => n.InnerText).ToList() ?? new List<string>();
            }

            private static List<string> ExtractComments(HtmlNode articleNode)
            {
                var nodes = articleNode.GetElementByClassName("comments")
                    .GetElementsByClassName("message html_format")
                    .SelectMany(n => n.ChildNodes).ToArray();

                foreach (var node in nodes.Where(node => node.Name != "#text"))
                {
                    node.Remove();
                }
                return nodes.Select(n => HtmlHelpers.ReplaceHtml(n.InnerText))
                    .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }

            private static int ExtractFavourites(HtmlNode articleNode)
            {
                return int.Parse(articleNode.GetElementByClassName("favorite-wjt__counter js-favs_count").InnerText);
            }

            private static int ExtractViews(HtmlNode articleNode)
            {
                return int.Parse(articleNode.GetElementByClassName("views-count_post").InnerText);
            }

            private static int ExtractRating(HtmlNode articleNode)
            {
                return int.Parse(articleNode.GetElementByClassName("voting-wjt__counter-score js-score").InnerText);
            }

            private static ICollection<string> ExtractTags(HtmlNode articleNode)
            {
                return
                    HtmlHelpers.ReplaceHtml(articleNode.GetElementByClassName("tags")
                        .InnerText).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            private static ICollection<string> ExtractHabs(HtmlNode articleNode)
            {

                return articleNode.GetElementByClassName("hubs")
                        .ChildNodes.Where(n => n.Name == "a")
                        .Select(n => n.InnerText).ToList();
            }

            private static DateTime ExtractDate(HtmlNode articleNode)
            {
                return HtmlHelpers.ParseHabrFormatDate(articleNode.GetElementByClassName("published").InnerText);
            }

            private static string ExtractName(HtmlNode articleNode)
            {
                return articleNode.GetElementByClassName("post_title").InnerText;
            }

            private static HtmlNode GetArticleNode(HtmlDocument html)
            {
                return html.DocumentNode.GetElementByClassName("post shortcuts_item");
            }

            private static HtmlDocument LoadHtmlDocument(string data)
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(data);
                return html;
            }
        }
    }
}