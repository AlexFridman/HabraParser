using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace HabraMiner
{
    public static class HtmlHelpers
    {
        public static ICollection<HtmlNode> GetElementsByClassName(this HtmlNode node, string className)
        {
            return node.SelectNodes($"//*[contains(@class,'{className}')]");
        }

        public static HtmlNode GetElementByClassName(this HtmlNode node, string className)
        {
            return node.GetElementsByClassName(className).FirstOrDefault();
        }

        public static DateTime ParseHabrFormatDate(string dateString)
        {
            return DateTime.ParseExact(dateString, "dd MMMM yyyy в hh:mm", new CultureInfo("ru-RU"));
        }
    }
}