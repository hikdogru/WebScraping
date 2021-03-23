using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebScraping.WebUI.Models
{
    public class ItemCheckedModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public bool IsCheck { get; set; }

    }
}