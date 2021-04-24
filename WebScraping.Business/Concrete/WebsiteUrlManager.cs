using System.Collections.Generic;
using WebScraping.Business.Abstract;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Business.Concrete
{
    public class WebsiteUrlManager:IWebsiteUrlService
    {
        private IWebsiteUrlRepository _websiteUrlRepository;

        public WebsiteUrlManager(IWebsiteUrlRepository websiteUrlRepository)
        {
            _websiteUrlRepository = websiteUrlRepository;
        }

        public List<WebsiteUrl> GetAll()
        {
            return _websiteUrlRepository.GetList();
        }

        public WebsiteUrl GetById(int websiteUrlId)
        {
            return _websiteUrlRepository.Get(w => w.Id == websiteUrlId);
        }

        public void Add(WebsiteUrl websiteUrl)
        {
            _websiteUrlRepository.Add(websiteUrl);
        }

        public void Update(WebsiteUrl websiteUrl)
        {
           _websiteUrlRepository.Update(websiteUrl);
        }

        public void Delete(int websiteUrlId)
        {
            var websiteUrl = _websiteUrlRepository.Get(w => w.Id == websiteUrlId);
            if (websiteUrl!= null)
            {
                _websiteUrlRepository.Delete(websiteUrl);
            }
        }
    }
}