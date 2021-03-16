using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping.Entity
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Price { get; set; }
        public string WebsiteName { get; set; }
        public Website Website { get; set; }
        public string Image { get; set; }
        public string BookDetailUrl { get; set; }
    }
}
