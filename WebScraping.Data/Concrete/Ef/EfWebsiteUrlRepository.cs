﻿using WebScraping.Core.Data.Ef;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class EfWebsiteUrlRepository: EfEntityRepositoryBase<WebsiteUrl, WebScrapingContext>, IWebsiteUrlRepository
    {
        
    }
}