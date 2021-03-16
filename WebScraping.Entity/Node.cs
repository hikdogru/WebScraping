using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraping.Entity
{
    public class Node
    {
        public HtmlNodeCollection Name { get; set; }
        public HtmlNodeCollection Price { get; set; }
        public HtmlNodeCollection Image { get; set; }
        public HtmlNodeCollection Detail { get; set; }

        public String WebsiteName { get; set; }
        public int CountItem { get; set; }

    }
}
