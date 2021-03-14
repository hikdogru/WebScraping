using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebScraping.WebUI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Publisher { get; set; }
        public string Price { get; set; }
        public string Website { get; set; }
        public string Image { get; set; }
    }
}