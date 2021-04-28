using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebScraping.Core.Data.Ef;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class EfWebsiteRepository: EfEntityRepositoryBase<Website, WebScrapingContext>, IWebsiteRepository
    {
        public List<Website> GetWebsiteWithNodeAndUrl()
        {
            return null;
        }
    }
}