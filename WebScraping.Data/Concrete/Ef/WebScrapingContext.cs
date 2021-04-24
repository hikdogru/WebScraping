using System.Data.Entity;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class WebScrapingContext:DbContext
    {
        public WebScrapingContext():base("WebScrapingConnection")
        {
            Database.SetInitializer(new DataInitializer());
        }

        public DbSet<Book> Books  { get; set; }
        public DbSet<BookNode> BookNodes { get; set; }
        public DbSet<WebsiteUrl> WebsiteUrls { get; set; }
        public DbSet<Website> Websites { get; set; }

    }
}