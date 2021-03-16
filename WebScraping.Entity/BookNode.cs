using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebScraping.Entity
{
    public class BookNode:Node
    {
        public HtmlNodeCollection Publisher { get; set; }
        
    }
}
