using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class Website:IEntity
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }

        public List<Book> Books { get; set; }
        public List<WebsiteUrl> WebsiteUrls { get; set; }
    }
}
