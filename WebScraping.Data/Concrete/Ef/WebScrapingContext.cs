using System.Data.Entity;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class WebScrapingContext:DbContext
    {
        public WebScrapingContext():base("WebScraping")
        {
            
        }

        public DbSet<Book> Books  { get; set; }
        public DbSet<BookNode> BookNodes { get; set; }
        public DbSet<WebsiteUrl> Urls { get; set; }
        public DbSet<Website> Websites { get; set; }

    }
}