﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class Book:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string BookDetailUrl { get; set; }
        // Best-seller or All-books
        public string CategoryType{ get; set; }

        public int WebsiteId { get; set; }
        public Website Website { get; set; }
    }
}
