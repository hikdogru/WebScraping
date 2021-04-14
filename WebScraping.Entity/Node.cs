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
        public HtmlNodeCollection Detail { get; set; }

        public String WebsiteName { get; set; }
        public int ItemCount { get; set; }

    }
}
