using System.Collections.Generic;
using WebScraping.Business.Abstract;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Business.Concrete
{
    public class WebsiteManager:IWebsiteService
    {
        private IWebsiteRepository _websiteRepository;

        public WebsiteManager(IWebsiteRepository websiteRepository)
        {
            _websiteRepository = websiteRepository;
        }

        public List<Website> GetAll()
        {
            return _websiteRepository.GetList();
        }

        public Website GetById(int websiteId)
        {
            return _websiteRepository.Get(w => w.Id == websiteId);
        }

        public void Add(Website website)
        {
            _websiteRepository.Add(website);
        }

        public void Update(Website website)
        {
            _websiteRepository.Update(website);
        }

        public void Delete(int websiteId)
        {
            var website = _websiteRepository.Get(w => w.Id == websiteId);
            if (website != null)
            {
                _websiteRepository.Delete(website);
            }
        }
    }
}