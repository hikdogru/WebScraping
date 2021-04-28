using System.Collections.Generic;
using WebScraping.Core.Data;
using WebScraping.Entities;

namespace WebScraping.Data.Abstract
{
    public interface IWebsiteRepository:IEntityRepository<Website>
    {
        List<Website> GetWebsiteWithNodeAndUrl();

    }
}