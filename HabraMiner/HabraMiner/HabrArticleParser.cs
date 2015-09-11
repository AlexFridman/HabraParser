using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HabraMiner
{
    class HabrArticleParser : IHabrArticleParser
    {
        public Article Parse(string data)
        {
            Article article = new Article();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(data);
            HtmlNode node = html.DocumentNode.GetElementByClassName("post shortcuts_item");


            return null;
        }

        
    }

    public static class HtmlHelpers
    {
        public static HtmlNode GetElementByClassName(this HtmlNode node, string className)
        {
            return node.SelectNodes($"//*[contains(@class,'{className}')]").FirstOrDefault();
        }
        
    }
}
