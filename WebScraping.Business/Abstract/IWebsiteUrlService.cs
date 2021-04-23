using System.Collections.Generic;
using WebScraping.Entities;

namespace WebScraping.Business.Abstract
{
    public interface IWebsiteUrlService
    {
        List<WebsiteUrl> GetAll();
        WebsiteUrl GetById(int websiteUrlId);
        void Add(WebsiteUrl websiteUrl);
        void Update(WebsiteUrl websiteUrl);
        void Delete(int websiteUrl);
    }
}