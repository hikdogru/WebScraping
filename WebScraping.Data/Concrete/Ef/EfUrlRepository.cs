using WebScraping.Core.Data.Ef;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class EfUrlRepository: EfEntityRepositoryBase<WebsiteUrl, WebScrapingContext>, IUrlRepository
    {
        
    }
}