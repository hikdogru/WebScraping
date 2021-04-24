using System.ComponentModel.DataAnnotations;
using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class Website:IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }

    }
}
