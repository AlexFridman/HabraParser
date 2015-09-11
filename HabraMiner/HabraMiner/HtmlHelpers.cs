using System.Linq;
using HtmlAgilityPack;

namespace HabraMiner
{
    public static class HtmlHelpers
    {
        public static HtmlNode GetElementByClassName(this HtmlNode node, string className)
        {
            return node.SelectNodes($"//*[contains(@class,'{className}')]").FirstOrDefault();
        }
    }
}