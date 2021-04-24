using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class WebsiteUrl : IEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string WebsiteName { get; set; }
        public Website Website { get; set; }

        // Best-seller or All-products
        public string UrlType { get; set; }
    }
}