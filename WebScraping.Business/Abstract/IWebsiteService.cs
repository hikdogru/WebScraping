using System.Collections.Generic;
using WebScraping.Entities;

namespace WebScraping.Business.Abstract
{
    public interface IWebsiteService
    {
        List<Website> GetAll();
        Website GetById(int websiteId);
        void Add(Website website);
        void Update(Website website);
        void Delete(int websiteId);
        List<Website> GetWebsiteWithNodeAndUrl();
    }
}