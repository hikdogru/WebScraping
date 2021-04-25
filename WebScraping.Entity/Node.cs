using HtmlAgilityPack;
using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class Node:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }
        public int ItemCount { get; set; }


        public int WebsiteId { get; set; }
        public Website Website { get; set; }
    }
}
