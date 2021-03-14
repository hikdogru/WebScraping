using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Scraping;

namespace WebScraping
{
    class Program
    {
        static List<Book> _books = new List<Book>();
        static void Main(string[] args)
        {
            //int n = 100;
            //List<string> websitesUrl = new List<string>()
            //{
            //    "https://www.bkmkitap.com/kitap/cok-satan-kitaplar/",
            //    "https://kidega.com/cok-satan-kitaplar/",
            //    "https://www.kitap16.com/cok-satanlar-kitapligi",
            //    "https://www.dr.com.tr/CokSatanlar/Kitap/",
            //    "https://www.ilknokta.com/cok-satanlar-b41.html",
            //    "https://www.eganba.com/kitap/cok-satanlar/",
            //};


            //foreach (var url in websitesUrl)
            //{
            //    HtmlWeb web = new HtmlWeb();
            //    HtmlDocument doc = web.Load(url);
            //    if (url.Contains("bkm"))
            //    {
            //        var bookNameNode = doc.DocumentNode.SelectNodes("//a[@class='fl col-12 text-description detailLink']");
            //        var bookPriceNode = doc.DocumentNode.SelectNodes("//div[@class='col col-12 currentPrice']");
            //        var bookPublisherNode = doc.DocumentNode.SelectNodes("//a[@class='col col-12 text-title mt']");
            //        string websiteName = "BKM Kitap";
            //        BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, websiteName, n);
            //    }

            //    else if (url.Contains("kidega"))
            //    {
            //        var bookNameNode = doc.DocumentNode.SelectNodes("//a[@class='book-name']");
            //        var bookPriceNode = doc.DocumentNode.SelectNodes("//b[@class='lastPrice']");
            //        var bookPublisherNode = doc.DocumentNode.SelectNodes("//a[@class='publisher']");
            //        string websiteName = "Kidega";
            //        BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, websiteName, n);
            //    }

            //    else if (url.Contains("kitap16"))
            //    {
            //        var bookNameNode = doc.DocumentNode.SelectNodes("//div[@class='name']");
            //        var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price price_sale convert_cur']");
            //        var bookPublisherNode = doc.DocumentNode.SelectNodes("//div[@class='publisher']/a");
            //        string websiteName = "Kitap16";
            //        BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, websiteName, n);
            //    }

            //    else if (url.Contains("dr"))
            //    {
            //        var bookNameNode = doc.DocumentNode.SelectNodes("//h3[@class='ellipsis']");
            //        var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price']");
            //        var bookPublisherNode = doc.DocumentNode.SelectNodes("//a[@class='who mb10']");
            //        string websiteName = "D&R";
            //        BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, websiteName, n);
            //    }

            //    else if (url.Contains("ilknokta"))
            //    {
            //        var bookNameNode = doc.DocumentNode.SelectNodes("//div[@class='name']/a");
            //        var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price price_sale convert_cur']");
            //        var bookPublisherNode = doc.DocumentNode.SelectNodes("//div[@class='publisher']");
            //        string websiteName = "İlk Nokta";
            //        BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, websiteName, n);
            //    }

            //    else if (url.Contains("eganba"))
            //    {
            //        var bookNameNode = doc.DocumentNode.SelectNodes("//a[@class='product-name']");
            //        var bookPriceNode = doc.DocumentNode.SelectNodes("//div[@class='product-price']/span/following-sibling::text()");
            //        var bookPublisherNode = doc.DocumentNode.SelectNodes("//div[@class='product-store']");
            //        string websiteName = "Eganba";
            //        BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, websiteName, n);
            //    }
            //}

            //Console.WriteLine();

            //var duplicates = _books.GroupBy(x => new {x.Name, x.Publisher})
            //    .Where(g => g.Count() > 3)
            //    .Select(y => y.Key)
            //    .ToList();
            //foreach (var duplicate in duplicates)
            //{
                

            //    var book = _books.Where(b => b.Name.Contains(duplicate.Name) && (b.Publisher.Contains(duplicate.Publisher))).ToList();

            //    Console.WriteLine($"   ----------{duplicate.Name}----------");
            //    Console.WriteLine();
            //    Console.WriteLine($"    Publisher |  Price");
            //    Console.WriteLine("    -------------------------------");
            //    for (int i = 0; i < book.Count; i++)
            //    {
            //        Console.WriteLine(String.Format("   {0,-10} | {1,-10} ", book[i].Website, book[i].Price));
            //        Console.WriteLine();
            //    }


            //}


            for (double i = 5.100011; i <= 5.1033; i += 0.000001)
            {
               
                if (Math.Pow(i,i)<=4096.01)      
                {
                    Console.WriteLine(i + " " + Math.Pow(i, i));
                }
            }

            
            Console.WriteLine();
            Console.ReadKey();
        }

        private static void BookScraping(HtmlNodeCollection bookNameNode, HtmlNodeCollection bookPriceNode, HtmlNodeCollection bookPublisherNode, string websiteName, int n)
        {
            int s = 0;
            var nodes = bookNameNode.Zip(bookPriceNode,
                (bookName, bookPrice) => new { bookName = bookName, bookPrice = bookPrice });
            var bookPublishers = new List<string>();

            foreach (var node in bookPublisherNode)
            {
                bookPublishers.Add(node.InnerText.Replace("\n", ""));
            }

            foreach (var selectNode in nodes)
            {
                s++;
                var bookName = System.Net.WebUtility.HtmlDecode(selectNode.bookName.InnerText.Replace("\n", "")).Trim();
                var bookPrice = selectNode.bookPrice.InnerText.Replace("\n", "")
                    .Replace("TL", " TL")
                    .Replace("₺", "TL")
                    .Replace("\r", "").
                    Replace(" ", "")
                    .Replace(" ", "TL")
                    .Trim();
                var publisher = System.Net.WebUtility.HtmlDecode(bookPublishers[s - 1]);
                var website = websiteName;
                var book = new Book() { Id = s, Name = bookName, Price = bookPrice, Publisher = publisher, Website = website };
                _books.Add(book);
                if (s == n)
                {
                    break;
                }
            }
        }
    }


}
