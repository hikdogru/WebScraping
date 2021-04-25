using HtmlAgilityPack;

namespace WebScraping.Entities
{
    public class BookNode : Node
    {
        public string Publisher { get; set; }

        public int WebsiteId { get; set; }
        public Website Website { get; set; }

    }
}
