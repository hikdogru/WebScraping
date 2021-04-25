using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class WebScrapingContext:DbContext
    {
        public WebScrapingContext():base("WebScrapingConnectionString")
        {
        }

        public DbSet<Book> Books  { get; set; }
        public DbSet<BookNode> BookNodes { get; set; }
        public DbSet<WebsiteUrl> WebsiteUrls { get; set; }
        public DbSet<Website> Websites { get; set; }

        

    }
}