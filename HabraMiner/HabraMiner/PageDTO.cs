using System;

namespace HabraMiner
{
    public class PageDTO
    {
        public PageDTO()
        {
            
        }
        public PageDTO(Uri uri, string content)
        {
            Uri = uri;
            Content = content;
        }

        public Uri Uri { get; set; }
        public string Content { get; set; } 
    }
}