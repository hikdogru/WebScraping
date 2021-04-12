﻿using HtmlAgilityPack;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebScraping.Entity;
using WebScraping.WebUI.Models;
using WebScraping.WebUI.ViewModel;

namespace WebScraping.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private static List<Book> _books = new List<Book>();
        private static List<Book> books;
        private static List<ItemCheckedModel> _itemCheckedModels = new List<ItemCheckedModel>();
        private List<string> _amazonBookDetails;

        private List<HtmlNodeCollection> BookDetailsNode = new List<HtmlNodeCollection>();
        private List<string> BookDetailUrl = new List<string>();
        private static RouteValueDictionary _routeValues;
        int pageSize = 20;
        private static int n = 1;

        Dictionary<string, string> _booksLogoUrl = new Dictionary<string, string>()
        {
            {"Bkm Kitap", "http://www.bkmkitap.com/Data/EditorFiles/logonew23.png"},
            {
                "Amazon",
                "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a9/Amazon_logo.svg/1920px-Amazon_logo.svg.png"
            },
            {"Kidega", "https://cdn.kidega.com/assets/web/img/kidega-logo.png"},
            {"Kitap16", "https://www.kitap16.com/u/kitap16/kitap-transparan-1579509339.png"},
            {"D&R", "https://www.dr.com.tr/Themes/DR/Content/assets/images/general/head-logo.png"},
            {"İlknokta", "https://www.ilknokta.com/u/ilknokta/ilknokta-logosu-1613392480.jpg"},
            {"Eganba", "https://www.eganba.com/wwwroot/images/eganba-logo.png"},
            {"Kitapseç", "https://cdn.kitapsec.com//temalar/KitapSec2017/img/logo.jpg"},
            {"Idefix", "https://fragtist.com/wp-content/uploads/2017/04/fragtist-IDEFIX-750x349.gif"},
            {"Fidan Kitap", "https://www.fidankitap.com/u/fidankitap/fidan-kitap-logo-9-1576766279.png"}
        };

        public ActionResult Index()
        {
            if (_books.Count > 0 == false)
            {
                Book();
            }

            return View(_books);
        }

        public ActionResult GetWebsite1(List<string> publisher, List<string> author, /*string publisher, string author,*/ int? minPrice, int? maxPrice, int? page = 1)
        {
            var itemViewModel = new ItemViewModel();
            itemViewModel.BookPerPage = 24;
            itemViewModel.CurrentPage = (int)page;
            IEnumerable<Book> filteredItems = books;
            if (publisher != null && publisher.Count > 0)
            {
                if (publisher[0].IndexOf(",") >= 0)
                {
                    var publisherId = publisher[0].Split(',').ToList();
                    var publishers = _itemCheckedModels.Where(m => publisherId.Any(p => int.Parse(p) == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = books.Where(b => publishers.Any(p => p == b.Publisher));
                }

                else
                {
                    var entities = _itemCheckedModels.Where(m => publisher.Any(p => int.Parse(p) == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = books.Where(b => entities.Any(p => p == b.Publisher));
                }

                publisher.ForEach(p => ViewBag.publisher += p + (publisher.Count > 1 ? "," : ""));
            }

            if (author != null && author.Count > 0)
            {
                if (author[0].IndexOf(",") >= 0)
                {
                    var authorId = author[0].Split(',').ToString();
                }
                var entities = _itemCheckedModels.Where(m => author.Any(a => int.Parse(a.Replace(",", "")) == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                filteredItems = filteredItems.Where(b => entities.Any(a => a == b.Author));
                author.ForEach(a => ViewBag.author += a + (author.Count > 1 ? "," : ""));
            }

            //if (!String.IsNullOrEmpty(author))
            //{
            //    var entity = _itemCheckedModels.Where(m => m.ItemEntityName == "Author" && m.ItemId == int.Parse(author)).GroupBy(m=>m.ItemName).Select(b=>b.Key);
            //    filteredItems = books.Where(b => entity.Any(p => p == b.Author));
            //    ViewBag.author = author;
            //}
            //if (!String.IsNullOrEmpty(publisher))
            //{
            //    var entity = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher" && m.ItemId == int.Parse(publisher)).GroupBy(m => m.ItemName).Select(b => b.Key);
            //    filteredItems = books.Where(b=> entity.Any(p => p == b.Publisher));
            //    ViewBag.publisher = publisher;
            //}

            itemViewModel.Books = filteredItems;
            //return PartialView("/Views/Shared/_GetProduct.cshtml", itemViewModel);
            return RedirectToAction("GetWebsite", "Home",
                new RouteValueDictionary(new { publisher = ViewBag.publisher, author = ViewBag.author, page = page }));

        }

        private void Book()
        {
            int n = 100;
            List<string> websitesUrl = new List<string>()
            {
                //"https://www.amazon.com.tr/gp/bestsellers/books/",
                //"https://www.amazon.com.tr/gp/bestsellers/books/ref=zg_bs_pg_2?ie=UTF8&pg=2",

                "https://www.bkmkitap.com/kitap/cok-satan-kitaplar/",
                "https://www.bkmkitap.com/kitap/cok-satan-kitaplar?pg=2/",

                "https://kidega.com/cok-satan-kitaplar/",
                "https://kidega.com/cok-satan-kitaplar?page=2/",

                "https://www.kitap16.com/cok-satanlar-kitapligi",
                "https://www.kitap16.com/index.php?p=Products&fpt_id=114&sort_type=rel-desc&page=2",

                "https://www.dr.com.tr/CokSatanlar/Kitap/",
                "https://www.dr.com.tr/CokSatanlar/Kitap/#/page=2/",

                "https://www.ilknokta.com/index.php?p=ProductBestsellers&mod_id=41&page=1&period=yearly",
                "https://www.ilknokta.com/index.php?p=ProductBestsellers&mod_id=41&page=2&period=yearly",

                "https://www.eganba.com/kitap/cok-satanlar/",
                "https://www.eganba.com/kitap/cok-satanlar/?page=2",

                "https://www.kitapsec.com/Products/Edebiyat/Cok-Satan-Kitaplar/",
                "https://www.kitapsec.com/Products/Edebiyat/Cok-Satan-Kitaplar/2-6-0a0-0-0-0-0-0.xhtml",

                "https://www.idefix.com/CokSatanlar/Kitap",
                "https://www.idefix.com/CokSatanlar/Kitap#/page=2",

                "https://www.fidankitap.com/cok-satanlar-2",
                "https://www.fidankitap.com/index.php?p=ProductBestsellers&mod_id=146&page=2"
            };
            foreach (var url in websitesUrl)
            {
                //if (url.Contains("amazon"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//span/div/span/a/div");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//span/div[@class='a-row']/a/span[@class='a-size-base a-color-price']/span[@class='p13n-sc-price']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='a-section a-spacing-small']/img");
                //    var bookDetailNode =
                //        doc.DocumentNode.SelectNodes(
                //            "//*[@class='aok-inline-block zg-item']/a[@class='a-link-normal']");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add("https://www.amazon.com.tr" + HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "Amazon";
                //    GetBookPublisher(BookDetailUrl, "Amazon");
                //    BookScraping(bookNameNode, bookPriceNode, null, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //    BookDetailsNode.Clear();
                //}

                if (url.Contains("bkm"))
                {
                    var bookNode = new BookNode()
                    {
                        Name = GetSelectNodes("//a[@class='fl col-12 text-description detailLink']", url),
                        Author = GetSelectNodes("//a[@class='fl col-12 text-title']", url),
                        Price = GetSelectNodes("//div[@class='col col-12 currentPrice']", url),
                        Image = GetSelectNodes("//span[@class='imgInner']/img", url),
                        Publisher = GetSelectNodes("//a[@class='col col-12 text-title mt']", url),
                        Detail = GetSelectNodes("//a[@class='fl col-12 text-description detailLink']", url),
                        WebsiteName = "BKM Kitap",
                        CountItem = n
                    };
                    foreach (var node in bookNode.Detail)
                    {
                        BookDetailUrl.Add("https://www.bkmkitap.com" +
                                          HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                    }

                    BookScraping(bookNode, "https://www.bkmkitap.com");
                }


                //else if (url.Contains("kidega"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//a[@class='book-name']");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//b[@class='lastPrice']");
                //    var bookPublisherNode = doc.DocumentNode.SelectNodes("//a[@class='publisher']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='image']/a/img/@src");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//a[contains(@class, 'book-name')]");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add(HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "Kidega";
                //    BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}

                //else if (url.Contains("kitap16"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//div[@class='name']");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price price_sale convert_cur']");
                //    var bookPublisherNode = doc.DocumentNode.SelectNodes("//div[@class='publisher']/a");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='image image_b']/a/img/@src");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'name')]/a");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add(HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "Kitap16";
                //    BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}

                //else if (url.Contains("dr"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//h3[@class='ellipsis']");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price']");
                //    var bookPublisherNode = doc.DocumentNode.SelectNodes("//a[@class='who mb10']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='content']/a/figure/img/@data-src");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//a[contains(@class, 'item-name')]");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add("https://www.dr.com.tr" + HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "D&R";
                //    BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}

                //else if (url.Contains("ilknokta"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//div[@class='name']/a");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price price_sale convert_cur']");
                //    var bookPublisherNode = doc.DocumentNode.SelectNodes("//div[@class='publisher']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='image image_b']/a/img/@src");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'name')]/a");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add("https://www.ilknokta.com" + HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "İlk Nokta";
                //    BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}

                //else if (url.Contains("eganba"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//a[@class='product-name']");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//div[@class='product-price']/span/following-sibling::text()");
                //    var bookPublisherNode = doc.DocumentNode.SelectNodes("//div[@class='product-store']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='prod-list-item']/div/a/img/@src");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//a[contains(@class, 'product-name')]");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add("https://www.eganba.com" + HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "Eganba";
                //    BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}
                //else if (url.Contains("idefix"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//div[@class='box-title']/a");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price price']");
                //    var bookPublisherNode = doc.DocumentNode.SelectNodes("//a[@class='who2 alternate']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//div[@class='image-area']/img");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'box-title')]/a");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add("https://www.idefix.com" + HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "Idefix";
                //    BookScraping(bookNameNode, bookPriceNode, bookPublisherNode, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}

                //else if (url.Contains("fidan"))
                //{
                //    var bookNameNode = doc.DocumentNode.SelectNodes("//div[@class='name']/a");
                //    var bookPriceNode = doc.DocumentNode.SelectNodes("//span[@class='price price_sale convert_cur']");
                //    var bookImageNode = doc.DocumentNode.SelectNodes("//a[@class='tooltip-ajax']/img");
                //    var bookDetailNode = doc.DocumentNode.SelectNodes("//div[contains(@class, 'writer')]/a");
                //    foreach (var node in bookDetailNode)
                //    {
                //        BookDetailUrl.Add("https://www.fidankitap.com" + HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //    }
                //    string websiteName = "Fidan Kitap";
                //    BookScraping(bookNameNode, bookPriceNode, null, bookImageNode, websiteName, n);
                //    BookDetailUrl.Clear();
                //}
            }
        }
        //private void BookScraping(HtmlNodeCollection bookNameNode, HtmlNodeCollection bookPriceNode, HtmlNodeCollection bookPublisherNode,
        //    HtmlNodeCollection bookImageNode, string websiteName, int n)
        //{
        //    int s = 0;
        //    var nodes = bookNameNode.Zip(bookPriceNode,
        //        (bookName, bookPrice) => new { bookName = bookName, bookPrice = bookPrice });
        //    var bookPublishers = new List<string>();
        //    var bookImages = new List<string>();
        //    if (websiteName == "Amazon")
        //    {
        //        if (BookDetailsNode.Count != 0)
        //        {
        //            foreach (var nodeCollection in BookDetailsNode)
        //            {
        //                bookPublishers.Add(nodeCollection[0].InnerText.Replace("\n", ""));
        //            }
        //        }
        //    }

        //    else if (bookPublisherNode != null)
        //    {
        //        foreach (var node in bookPublisherNode)
        //        {
        //            bookPublishers.Add(node.InnerText.Replace("\n", ""));
        //        }
        //    }

        //    foreach (var node in bookImageNode)
        //    {
        //        if (websiteName == "Amazon")
        //        {
        //            bookImages.Add(node.GetAttributeValue("src", ""));

        //        }
        //        else
        //        {
        //            bookImages.Add(node.GetAttributeValue("data-src", ""));
        //        }
        //    }
        //    foreach (var selectNode in nodes)
        //    {

        //        s++;
        //        var bookName = WebUtility.HtmlDecode(selectNode.bookName.InnerText.Replace("\n", "")).Trim();
        //        var bookPrice = selectNode.bookPrice.InnerText.Replace("\n", "")
        //            .Replace("TL", " TL")
        //            .Replace("₺", "TL")
        //            .Replace("\r", "")
        //            .Replace(" ", "")
        //            .Replace(" ", "TL")
        //            .Trim();
        //        var publisher = "";
        //        if (bookPublishers.Count != 0)
        //        {
        //            publisher = WebUtility.HtmlDecode(bookPublishers[s - 1]);
        //        }


        //        var image = WebUtility.HtmlDecode(bookImages[s - 1]);
        //        var website = websiteName;
        //        var book = new Book() { Id = s, Name = bookName, Price = bookPrice, Publisher = publisher, Website = website, Image = image, BookDetailUrl = BookDetailUrl[s - 1] };

        //        _books.Add(book);


        //        if (s == n)
        //        {
        //            break;
        //        }
        //    }
        //}

        private void BookScraping(BookNode bookNode, string websiteUrl)
        {
            for (int i = 0; i < bookNode.Publisher.Count; i++)
            {
                string bookName = WebUtility.HtmlDecode(bookNode.Name[i].InnerText.Replace("\n", "")).Trim();
                string author = WebUtility.HtmlDecode(bookNode.Author[i].InnerText);
                string bookPrice = bookNode.Price[i].InnerText.Replace("\n", "")
                    .Replace("TL", " TL")
                    .Replace("₺", "TL")
                    .Replace("\r", "")
                    .Replace(" ", "")
                    .Replace(" ", "TL")
                    .Trim();
                string publisher = WebUtility.HtmlDecode(bookNode.Publisher[i].InnerText);
                string image = WebUtility.HtmlDecode(bookNode.Image[i].GetAttributeValue("data-src", ""));
                string website = bookNode.WebsiteName;
                string bookDetailUrl = websiteUrl +
                                       HttpUtility.UrlDecode(bookNode.Detail[i]
                                           .GetAttributeValue("href", string.Empty));
                var book = new Book()
                {
                    Id = i + 1,
                    Name = bookName,
                    Author = author,
                    Price = bookPrice,
                    Publisher = publisher,
                    WebsiteName = website,
                    Image = image,
                    BookDetailUrl = bookDetailUrl
                };
                _books.Add(book);
                if (i == bookNode.CountItem)
                {
                    break;
                }
            }
        }

        private HtmlNodeCollection GetSelectNodes(string xpath, string websiteUrl)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(websiteUrl);
            var node = doc.DocumentNode.SelectNodes(xpath);
            return node;
        }


        public ActionResult GetWebsite(List<string> publisher, List<string> author, int? minPrice = 0, int? maxPrice = 0, int? page = 1)
        {
            var itemViewModel = new ItemViewModel();
            itemViewModel.BookPerPage = 24;
            itemViewModel.CurrentPage = (int)page;
            IEnumerable<Book> filteredItems = books;
            if (minPrice != 0 && maxPrice != 0)
            {
                filteredItems = filteredItems.Where(b =>
                    double.Parse(b.Price.Trim()) >= minPrice && double.Parse(b.Price.Trim()) <= maxPrice);
                itemViewModel.Books = filteredItems;
                ViewBag.minPrice = minPrice;
                ViewBag.maxPrice = maxPrice;
            }

            if (publisher != null && publisher.Count > 0)
            {
                if (publisher[0].IndexOf(",") >= 0)
                {
                    var publisherId = publisher[0].Split(',').ToList();
                    var publishers = _itemCheckedModels.Where(m => publisherId.Any(p => int.Parse(!String.IsNullOrEmpty(p) ? p : "0") == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => publishers.Any(p => p == b.Publisher));
                    ViewBag.publisherId = publisherId;
                }

                else
                {
                    var entities = _itemCheckedModels.Where(m => publisher.Any(p => int.Parse(p.Replace(",", "")) == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => entities.Any(p => p == b.Publisher));
                    ViewBag.publisherId = publisher;
                }

                publisher.ForEach(p => ViewBag.publisher += p + (publisher.Count > 1 ? "," : ""));

            }

            if (author != null && author.Count > 0)
            {
                if (author[0].IndexOf(",") >= 0)
                {
                    var authorId = author[0].Split(',').ToList();
                    var authors = _itemCheckedModels.Where(m => authorId.Any(a => int.Parse(!String.IsNullOrEmpty(a) ? a : "0") == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => authors.Any(a => a == b.Author));
                    ViewBag.authorId = authorId;
                }
                else
                {
                    var entities = _itemCheckedModels.Where(m => author.Any(a => int.Parse(a.Replace(",", "")) == m.ItemId)).GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => entities.Any(a => a == b.Author));
                    ViewBag.authorId = author;
                }

                author.ForEach(a => ViewBag.author += a + (author.Count > 1 ? "," : ""));
            }

            itemViewModel.Books = filteredItems;
            GetItemsByChecked();

            // checkbox publishers value
            TempData["Publishers"] = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");

            // checkbox authors value
            TempData["Authors"] = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");

            if (Request.IsAjaxRequest())
            {
                return PartialView("/Views/Shared/_GetProduct.cshtml", itemViewModel);
            }
            else
            {
                return View(itemViewModel);
            }
        }

        private static void GetItemsByChecked()
        {
            if (_itemCheckedModels.Count == 0)
            {
                var publishers = books.OrderBy(b=>b.Publisher).GroupBy(b => b.Publisher).Select(b => b.Key).Distinct();
                var authors = books.OrderBy(b=>b.Author).GroupBy(b => b.Author).Select(b => b.Key).Distinct();
                int itemId = 1;
                foreach (var publisherName in publishers)
                {
                    var itemCheckedModel = new ItemCheckedModel();
                    itemCheckedModel.ItemEntityName = "Publisher";
                    itemCheckedModel.IsCheck = false;
                    itemCheckedModel.ItemName = publisherName;
                    itemCheckedModel.ItemId = itemId;
                    _itemCheckedModels.Add(itemCheckedModel);
                    itemId++;
                }

                foreach (var author in authors)
                {
                    var itemCheckedModel = new ItemCheckedModel();
                    itemCheckedModel.ItemEntityName = "Author";
                    itemCheckedModel.IsCheck = false;
                    itemCheckedModel.ItemName = author;
                    itemCheckedModel.ItemId = itemId;
                    _itemCheckedModels.Add(itemCheckedModel);
                    itemId++;
                }
            }
        }

        [HttpPost]
        public ActionResult GetWebsite(string website, int page = 1)
        {
            books = new List<Book>();
            List<string> websites = website.Split(',').ToList();
            IEnumerable<Book> distinct;
            foreach (var i in websites)
            {
                //if (i == "Amazon")
                //{
                //    distinct = _books.Where(b => b.WebsiteName == i).GroupBy(b => b.Name).Select(b => b.First());
                //}


                distinct = _books.Where(b => b.WebsiteName == i).GroupBy(b => b.Image).Select(b => b.First());
                foreach (var book in distinct)
                {
                    books.Add(book);
                }
            }

            TempData["WebsiteBooks"] = books;
            TempData["BooksLogoUrl"] = _booksLogoUrl;
            var booksView = new ItemViewModel()
            {
                BookPerPage = 24,
                Books = books,
                CurrentPage = page
            };

            TempData["BooksView"] = booksView;
            GetItemsByChecked();
            ViewBag.Publishers = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");
            ViewBag.Authors = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");

            return View(booksView);
        }

        [HttpPost]
        public ActionResult GetFilteredItems(int? minPrice, int? maxPrice)
        {
            string url = "";
            IEnumerable<Book> filteredItems;
            for (int i = 0; i < books.Count; i++)
            {
                books[i].Price = books[i].Price.Replace("TL", "");
                books[i].Price = books[i].Price.Replace(",", ".");
            }


            if (minPrice == maxPrice)
            {
                filteredItems = books.Where(b => double.Parse(b.Price.Trim()) == minPrice);
            }
            else
            {
                filteredItems = books.Where(b =>
                    double.Parse(b.Price.Trim()) >= minPrice && double.Parse(b.Price.Trim()) <= maxPrice);
            }

            if (_routeValues != null)
            {
                filteredItems = filteredItems.Where(b => _routeValues.Values.Any(p => p.ToString() == b.Publisher));
                TempData["RouteValueDictionary"] = _routeValues;
                for (int i = 1; i <= _routeValues.Count; i++)
                {
                    string routeValues = _routeValues["publisher[" + i + "]"].ToString();
                    routeValues = routeValues.Replace(" ", "-");
                    url += routeValues + "&";
                }
            }

            var itemViewModel = new ItemViewModel()
            {
                BookPerPage = 24,
                Books = filteredItems,
                CurrentPage = 1
            };

            TempData["ItemViewModel"] = itemViewModel;
            TempData["minPrice"] = minPrice;
            TempData["maxPrice"] = maxPrice;
            return RedirectToAction("GetWebsite", "Home",
                new RouteValueDictionary(new { url, minPrice = minPrice, maxPrice = maxPrice }));
        }

        [HttpPost]
        public ActionResult Search(string query)
        {
            if (!String.IsNullOrEmpty(query))
            {
                var otherPublishers = _books.Where(b => b.Name.ToUpper()
                        .Contains(query.ToUpper()) && b.Publisher != "")
                    .GroupBy(b => new { b.Publisher, b.Name })
                    .Select(b => b.FirstOrDefault())
                    .OrderBy(b => ReplaceString(b.Price)).ToList();


                if (otherPublishers.Count > 1)
                {
                    TempData["OtherPublishers"] = otherPublishers;
                }

                var distinct = _books.Where(b => b.Name.ToUpper().Trim().Contains(query.ToUpper()))
                    .OrderBy(b => ReplaceString(b.Price)).DistinctBy(b => b.Id).ToList();
                TempData["Books"] = distinct;
                TempData["BooksLogoUrl"] = _booksLogoUrl;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetBooksByWebsitePrice(string bookName, string bookPublisher)
        {
            var entities = _books.Where(b => b.Name == bookName && b.Publisher.Contains(bookPublisher))
                .GroupBy(b => b.Website)
                .Select(b => b.FirstOrDefault());
            TempData["Entities"] = entities.ToList();
            TempData["BooksLogoUrl"] = _booksLogoUrl;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetBooksByPublisher(string publisher, int? publisherId)
        {
            IEnumerable<Book> booksByPublisher;
            int minPrice = 0;
            int maxPrice = 0;
            string publisherUrl = "";
            if (TempData["minPrice"] != null && TempData["maxPrice"] != null)
            {
                minPrice = int.Parse(TempData["minPrice"].ToString());
                maxPrice = int.Parse(TempData["maxPrice"].ToString());
            }

            if (publisher != null)
            {
                var entity = _itemCheckedModels.Where(m =>
                        m.ItemId == publisherId && m.ItemName == publisher && m.ItemEntityName == "Publisher")
                    .FirstOrDefault();
                if (entity != null)
                {
                    if (entity.IsCheck)
                    {
                        entity.IsCheck = false;
                    }
                    else
                    {
                        entity.IsCheck = true;
                    }
                }

                var checkedPublishers =
                    _itemCheckedModels.Where(m => m.IsCheck && m.ItemEntityName == "Publisher").GroupBy(m => m.ItemName)
                        .Select(m => m.Key);
                booksByPublisher = books.Where(b => checkedPublishers.Any(m => m == b.Publisher));
                if (minPrice != 0 && maxPrice != 0)
                {
                    booksByPublisher = booksByPublisher.Where(b =>
                        double.Parse(b.Price.Trim()) >= minPrice && double.Parse(b.Price.Trim()) <= maxPrice);
                }

                var itemViewModel = new ItemViewModel() { BookPerPage = 24, Books = booksByPublisher, CurrentPage = 1 };
                TempData["ItemViewModel"] = itemViewModel;
                _routeValues = new RouteValueDictionary();
                for (int i = 1; i <= checkedPublishers.Count(); i++)
                {
                    _routeValues["publisher[" + i + "]"] = checkedPublishers.ToList()[i - 1];
                }

                for (int i = 1; i <= _routeValues.Count; i++)
                {
                    string routeValues = _routeValues["publisher[" + i + "]"].ToString();
                    routeValues = routeValues.Replace(" ", "-");
                    publisherUrl += "publisher" + i + "=" + routeValues + (i != _routeValues.Count ? "-" : "");
                }

                TempData["RouteValueDictionary"] = _routeValues;
            }



            return RedirectToAction("GetWebsite", "Home", new RouteValueDictionary(new { publisher = publisher }));
            //return RedirectToAction("GetWebsite", "Home",
            //    new RouteValueDictionary(_routeValues));
        }
        [HttpGet]
        public ActionResult GetBooksByAuthor(string author, int? authorId)
        {
            var entity = _itemCheckedModels
                .Where(m => m.ItemName == author && m.ItemId == authorId && m.ItemEntityName == "Author")
                .FirstOrDefault();
            if (entity != null)
            {
                if (entity.IsCheck)
                {
                    entity.IsCheck = false;
                }
                else
                {
                    entity.IsCheck = true;
                }
            }

            var checkedAuthors = _itemCheckedModels.Where(m => m.IsCheck && m.ItemEntityName == "Author")
                .GroupBy(m => m.ItemName).Select(m => m.Key);
            var booksByAuthor = books.Where(b => checkedAuthors.Any(a => a == b.Author));
            var itemViewModel = new ItemViewModel() { BookPerPage = 24, Books = booksByAuthor, CurrentPage = 1 };
            TempData["ItemViewModel"] = itemViewModel;

            //return Redirect("/Home/GetWebsite?" + url);
            return RedirectToAction("GetWebsite", "Home", new RouteValueDictionary(new { author = author }));
        }


        // Tl ekini kaldırıyor.
        public static int ReplaceString(string price)
        {
            string bookPrice = price.Replace("TL", "");
            return (int)float.Parse(bookPrice);
        }

        public void GetBookPublisher(List<string> urls, string websiteName)
        {
            HtmlWeb web = new HtmlWeb();
            using (var client = new WebClient())
            {
                //Türkçe karakter sorunu yapmaması için encoding utf8 yapıyoruz
                client.Encoding = Encoding.UTF8;
                foreach (var url in urls)
                {
                    var document = new HtmlDocument();
                    string html = client.DownloadString(url);
                    document.LoadHtml(html);
                    if (websiteName == "Amazon")
                    {
                        BookDetailsNode.Add(document.DocumentNode.SelectNodes(
                            "//div[@id='detailBullets_feature_div']/ul/li[1]/span[@class='a-list-item']/span[2]"));
                    }
                }
            }


            //var document = web.Load(link);

            //if (websiteName == "Amazon")
            //{
            //    BookDetailsNode.Add(document.DocumentNode.SelectNodes(
            //        "//div[@id='detailBullets_feature_div']/ul/li[1]/span[@class='a-list-item']/span[2]"));
            //}
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}