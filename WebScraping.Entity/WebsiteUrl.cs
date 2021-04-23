using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class WebsiteUrl : IEntity
    {
        public string Url { get; set; }
        public string WebsiteName { get; set; }

        // Best-seller or All-products
        public string UrlType { get; set; }
    }
}