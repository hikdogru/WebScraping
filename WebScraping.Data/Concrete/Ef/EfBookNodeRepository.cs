using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebScraping.Core.Data.Ef;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class EfBookNodeRepository:EfEntityRepositoryBase<BookNode, WebScrapingContext>, IBookNodeRepository
    {
        public List<BookNode> GetNodesByWebsite()
        {
            using (var context = new WebScrapingContext())
            {
                var nodes = context.BookNodes.Include(w => w.Website).Include(w => w.Website.WebsiteUrls).ToList();
                return nodes;
            }
        }
    }
}