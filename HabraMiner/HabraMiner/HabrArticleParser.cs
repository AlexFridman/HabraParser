using System;
using System.Collections.Generic;
using System.Linq;
using HabraMiner.Interfaces;
using HtmlAgilityPack;

namespace HabraMiner
{
    class HabrArticleParser : IHabrArticleParser
    {
        public Article Parse(string data)
        {
            Article article = new Article();

            var html = LoadHtmlDocument(data);

            HtmlNode articleNode = GetArticleNode(html);
            article.Name = ExtractName(articleNode);
            article.Date = ExtractDate(articleNode);
            article.Habs = ExtractHabs(articleNode);
            article.Tags = ExtractTags(articleNode);
            article.Rating = ExtractRating(articleNode);
            article.Views = ExtractViews(articleNode);
            article.Favourites = ExtractFavourites(articleNode);
            //TODO check other comments
            article.CodeComments = ExtractCodeComments(articleNode);
            article.Author = ExtractAuthor(articleNode);

            DeleteImageNodes(articleNode);

            DeleteCodeNodes(articleNode);
            var articleComments = ExtractArticleComments(html);

            // this is html, not text
            // TODO parse Text
            article.Text = articleNode.GetElementByAttributeValue("content html_format").InnerText;

            return article;
        }

        private ICollection<string> ExtractArticleComments(HtmlDocument html)
        {
            return HtmlHelpers.GetElementsByClassName(html, "message html_format ");
        }

        private static void DeleteCodeNodes(HtmlNode articleNode)
        {
            var codeNotes = articleNode.GetElementsByAttributeValue("nginx");
            foreach (var node in codeNotes)
            {
                node.Remove();
            }
            codeNotes = articleNode.GetElementsByAttributeValue("bash");
            foreach (var node in codeNotes)
            {
                node.Remove();
            }
        }

        private static void DeleteImageNodes(HtmlNode articleNode)
        {
            var imageNotes = articleNode.GetElementsByClassName("img");
            foreach (var image in imageNotes)
            {
                image.Remove();
            }
        }

        private static string ExtractAuthor(HtmlNode articleNode)
        {
            return articleNode.GetElementByAttributeValue("author-info__name").InnerText;
        }

        private static List<string> ExtractCodeComments(HtmlNode articleNode)
        {
            return articleNode.GetElementsByAttributeValue("nginx").Select(n => n.InnerText).ToList();
        }

        private static int ExtractFavourites(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByAttributeValue("favorite-wjt__counter js-favs_count").InnerText);
        }

        private static int ExtractViews(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByAttributeValue("views-count_post").InnerText);
        }

        private static int ExtractRating(HtmlNode articleNode)
        {
            return int.Parse(articleNode.GetElementByAttributeValue("voting-wjt__counter-score js-score").InnerText);
        }

        private static ICollection<string> ExtractTags(HtmlNode articleNode)
        {
            return 
                articleNode.GetElementByAttributeValue("tags")
                    .GetElementsByAttributeValue("tag")
                    .Select(n => n.InnerText).ToList();
        }

        private static ICollection<string> ExtractHabs(HtmlNode articleNode)
        {

            return articleNode.GetElementByAttributeValue("hubs")
                    .ChildNodes.Where(n => n.Name == "a")
                    .Select(n => n.InnerText).ToList();
        }

        private static DateTime ExtractDate(HtmlNode articleNode)
        {
            return HtmlHelpers.ParseHabrFormatDate(articleNode.GetElementByAttributeValue("published").InnerText);
        }

        private static string ExtractName(HtmlNode articleNode)
        {
            return articleNode.GetElementByAttributeValue("post_title").InnerText;
        }

        private static HtmlNode GetArticleNode(HtmlDocument html)
        {
            return html.DocumentNode.GetElementByAttributeValue("post shortcuts_item");
        }

        private static HtmlDocument LoadHtmlDocument(string data)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(data);
            return html;
        }
    }
}
