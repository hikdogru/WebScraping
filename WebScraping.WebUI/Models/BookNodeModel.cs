using HtmlAgilityPack;

namespace WebScraping.WebUI.Models
{
    public class BookNodeModel
    {
        public int Id { get; set; }
        public HtmlNodeCollection Name { get; set; }
        public HtmlNodeCollection Author { get; set; }
        public HtmlNodeCollection Publisher { get; set; }
        public HtmlNodeCollection Price { get; set; }
        public HtmlNodeCollection Image { get; set; }
        public HtmlNodeCollection Detail { get; set; }
        public int ItemCount { get; set; }


        public int WebsiteId { get; set; }
    }
}