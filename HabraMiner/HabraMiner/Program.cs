using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabraMiner.Articles;
using HabraMiner.Exceptions;
using NLog;

namespace HabraMiner
{
    class Program
    {
        private static void Main(string[] args)
        {
            try{ 
            var article = HabrArticle.HabrParser.Parse(File.ReadAllText(@"D:\temp\64931.htm"));
            Console.WriteLine(article.Name);
        }

    catch (NotFoundException ex)
            {
                Console.WriteLine("foo");
                //Logger.Error($"Unsuccessful post downloading (404) {uri.AbsolutePath}");
            }
        }
    }
}
