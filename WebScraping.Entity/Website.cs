using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class Website : IEntity
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }

    }
}
