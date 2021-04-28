using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebScraping.Core.Entities;

namespace WebScraping.Entities
{
    public class WebsiteUrl : IEntity
    {
        public int Id { get; set; }
        public string Url { get; set; }
        // Best-seller or All-products
        public string UrlType { get; set; }
        public int WebsiteId { get; set; }
        public Website Website { get; set; }
    }
}