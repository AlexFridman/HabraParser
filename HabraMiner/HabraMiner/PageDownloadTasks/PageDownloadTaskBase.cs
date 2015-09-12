﻿using System;
using System.Threading.Tasks;

namespace HabraMiner.PageDownloadTasks
{
    public class PageDownloadTaskBase
    {
        public Uri Uri { get; set; }
        public Task<string> DownloadTask { get; set; } 
    }
}