using HtmlAgilityPack;
using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class Node:IEntity
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
