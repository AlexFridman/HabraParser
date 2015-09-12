using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HabraMiner.Interfaces;
using HtmlAgilityPack;

namespace HabraMiner
{
    class HabrArticleParser : IHabrArticleParser
    {
        public Article Parse(string data)
        {
            var html = LoadHtmlDocument(data);

            var article = new Article();

            var articleNode = GetArticleNode(html);
            article.Name = ExtractName(articleNode);
            article.Date = ExtractDate(articleNode);
            article.Hubs = ExtractHabs(articleNode);
            article.Tags = ExtractTags(articleNode);
            article.Rating = ExtractRating(articleNode);
            article.Views = ExtractViews(articleNode);
            article.Favourites = ExtractFavourites(articleNode);
            article.CodeComments = ExtractCodeComments(articleNode);
            article.Author = ExtractAuthor(articleNode);

            DeleteImageNodes(articleNode);

            DeleteCodeNodes(articleNode);
            article.Text = articleNode.GetElementByAttributeValue("content html_format").InnerText;
            
            article.Comments = ExtractComments(articleNode);

            return article;
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
            return HtmlHelpers.ReplaceHtml(articleNode.GetElementByAttributeValue("author-info__name").InnerText);
        }

        private static List<string> ExtractCodeComments(HtmlNode articleNode)
        {
            return articleNode.GetElementsByAttributeValue("nginx").Select(n => n.InnerText).ToList();
        }

        private static List<string> ExtractComments(HtmlNode articleNode)
        {
            var nodes = articleNode.GetElementByAttributeValue("comments")
                .GetElementsByAttributeValue("message html_format")
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
                HtmlHelpers.ReplaceHtml(articleNode.GetElementByAttributeValue("tags")
                    .InnerText).Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries).ToList();
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
