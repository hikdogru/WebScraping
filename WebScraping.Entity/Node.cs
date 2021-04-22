using HtmlAgilityPack;
using System;

namespace WebScraping.Entity
{
    public class Node
    {
        public HtmlNodeCollection Name { get; set; }
        public HtmlNodeCollection Author { get; set; }
        public HtmlNodeCollection Price { get; set; }
        public HtmlNodeCollection Image { get; set; }
        public string Detail { get; set; }

        public string WebsiteName { get; set; }
        public int ItemCount { get; set; }

    }
}
