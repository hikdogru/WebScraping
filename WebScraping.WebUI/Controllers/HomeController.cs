using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HtmlAgilityPack;
using WebScraping.Business.Abstract;
using WebScraping.Entities;
using WebScraping.WebUI.Models;
using WebScraping.WebUI.ViewModel;

namespace WebScraping.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private static readonly List<Book> BestSellerBooks = new List<Book>();
        private static readonly List<Book> AllBooks = new List<Book>();
        private static List<Book> books;
        private static readonly List<ItemCheckedModel> ItemCheckedModels = new List<ItemCheckedModel>();
        private readonly List<string> _bookDetailUrl = new List<string>();
        private List<Book> filteredItems;

        private static readonly Dictionary<string, string> BooksLogoUrl = new Dictionary<string, string>();

        private readonly IBookNodeService _bookNodeService;
        private IWebsiteService _websiteService;
        private readonly IWebsiteUrlService _websiteUrlService;
        private readonly IBookService _bookService;


        public HomeController(IWebsiteService websiteService, IWebsiteUrlService websiteUrlService,
            IBookNodeService bookNodeService, IBookService bookService)
        {
            _websiteService = websiteService;
            _websiteUrlService = websiteUrlService;
            _bookNodeService = bookNodeService;
            _bookService = bookService;
        }


        public ActionResult Index()
        {

            if (BestSellerBooks.Count > 0 == false) Book();
            return View(BestSellerBooks);
        }


        private void Book()
        {
            var bookNodes = _bookNodeService.GetNodesByWebsite();
            var bestSellerBooks = _bookService.GetBooksWithWebsite().Where(b => b.CategoryType == "Best-seller");
            var allBooks = _bookService.GetBooksWithWebsite().Where(b => b.CategoryType == "All-books");
            var websites = _websiteService.GetAll();
            BestSellerBooks.AddRange(bestSellerBooks);
            AllBooks.AddRange(allBooks);
            foreach (var website in websites)
            {
                BooksLogoUrl.Add(website.Name, website.LogoUrl);
            }


            //foreach (var node in bookNodes)
            //{
            //    if (node.WebsiteId != 2 && node.WebsiteId != 11 && node.Website.Id != 11 && node.Website.Id != 2)
            //    {
            //        foreach (var websiteUrl in node.Website.WebsiteUrls)
            //        {
            //            var bookDetailNode = SelectNodes(node.Detail, websiteUrl.Url);
            //            foreach (var detailNode in bookDetailNode)
            //            {

            //                _bookDetailUrl.Add((detailNode.GetAttributeValue("href", string.Empty).Contains("http")
            //                                       ? ""
            //                                       : node.Website.WebsiteUrl) +
            //                                   HttpUtility.UrlDecode(
            //                                       detailNode.GetAttributeValue("href", string.Empty)));
            //            }

            //            var bookNodeXpath = new BookNodeXPath()
            //            {
            //                Name = node.Name,
            //                Author = node.Author,
            //                Price = node.Price,
            //                Image = node.Image,
            //                Publisher = node.Publisher,
            //            };

            //            var bookNode = SelectNodes(bookNodeXpath, websiteUrl.Url);

            //            BookScraping(bookNode, node.Website.WebsiteUrl, node.WebsiteId, websiteUrl.UrlType);

            //            _bookDetailUrl.Clear();
            //        }
            //    }
            //}
        }





        private void BookScraping(BookNodeModel bookNode, string websiteUrl, int websiteId, string type = "")
        {
            var bookPrice = "";

            for (var i = 0; i < bookNode.Author.Count; i++)
            {
                var bookName = WebUtility
                    .HtmlDecode(bookNode.Name == null ? "" : bookNode.Name[i].InnerText.Replace("\n", "")).Trim();
                var author = WebUtility.HtmlDecode(bookNode.Author[i].InnerText.Replace("\n", "")).Trim();
                var publisher = WebUtility.HtmlDecode(bookNode.Publisher[i].InnerText);
                var image = WebUtility.HtmlDecode(bookNode.Image[i].GetAttributeValue("data-src", "") != ""
                    ? bookNode.Image[i].GetAttributeValue("data-src", "")
                    : bookNode.Image[i].GetAttributeValue("src", ""));

                if (websiteUrl.Contains("hepsiburada"))
                {
                    var bookNameAndAuthor = bookName.Replace(" – ", "*").Replace("-", "*").Split('*');
                    bookName = bookNameAndAuthor[0];
                    author = bookNameAndAuthor.Length > 1 ? bookNameAndAuthor[1] : "";
                    bookPrice = bookNode.Price[i].GetAttributeValue("content", string.Empty);
                    image = WebUtility.HtmlDecode(bookNode.Image[i].GetAttributeValue("src", ""));
                }
                else
                {
                    if (bookNode.Price == null)
                        bookPrice = "0";
                    else
                        bookPrice = bookNode.Price[i].InnerText.Replace("\n", "")
                            .Replace("TL", " TL")
                            .Replace("₺", "TL")
                            .Replace("\r", "")
                            .Replace(" ", "")
                            .Replace(" ", "TL")
                            .Trim();
                }


                var book = new Book
                {
                    Id = i + 1,
                    Name = bookName,
                    Author = author,
                    Price = bookPrice,
                    Publisher = publisher,
                    WebsiteId = websiteId,
                    Image = image,
                    BookDetailUrl = _bookDetailUrl[i],
                    CategoryType = type


                };

                if (_bookService.Add(book))
                {
                    _bookService.Add(book);
                }
                else
                {
                    var entity = _bookService.GetAll().Where(b => b.BookDetailUrl == book.BookDetailUrl).FirstOrDefault(); 
                    _bookService.Update(entity);
                }


                if (i == bookNode.ItemCount) break;
            }
        }


        private HtmlNodeCollection SelectNodes(string xpath, string websiteUrl)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            using (var myWebClient = new WebClient())
            {
                myWebClient.Headers["User-Agent"] =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                myWebClient.Headers["Accept"] =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                var page = myWebClient.DownloadData(websiteUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(Encoding.UTF8.GetString(page));
                var node = doc.DocumentNode.SelectNodes(xpath);
                watch.Stop();
                ViewBag.SelectNodes1 = watch.ElapsedMilliseconds;
                return node;
            }

        }

        private BookNodeModel SelectNodes(BookNodeXPath bookNodeXPath, string websiteUrl)
        {

            using (var myWebClient = new WebClient())
            {
                myWebClient.Headers["User-Agent"] =
                    "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                myWebClient.Headers["Accept"] =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                var page = myWebClient.DownloadData(websiteUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(Encoding.UTF8.GetString(page));
                var nameNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Name);
                var authorNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Author);
                var publisherNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Publisher);
                var imageNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Image);
                var priceNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Price);

                var bookNode = new BookNodeModel
                {
                    Name = nameNode,
                    Author = authorNode,
                    Price = priceNode,
                    Image = imageNode,
                    Publisher = publisherNode,
                    Detail = bookNodeXPath.Detail,
                    ItemCount = 200,

                };



                return bookNode;
            }
        }


        public ActionResult GetWebsite(List<string> website, List<string> publisher, List<string> author, int? minPrice = 0,
                int? maxPrice = 0, int? page = 1, string sort = "")
        {
            var itemViewModel = new ItemViewModel();
            itemViewModel.BookPerPage = 24;
            itemViewModel.CurrentPage = (int)page;
            filteredItems = books;
            if (minPrice != 0 && maxPrice != 0)
            {
                itemViewModel.Books = FilteringByPrice(minPrice, maxPrice); 
            }

            if (website != null && website.Count > 0)
            {
                if (website[0].IndexOf(',') >= 0)
                {
                    var websiteIds = website[0].Split(',').ToList();
                    var websites = ItemCheckedModels
                        .Where(m => websiteIds.Any(w => int.Parse(!string.IsNullOrEmpty(w) ? w : "0") == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => websites.Any(w => w == b.Website.Name)).ToList();
                    ViewBag.WebsiteId = websiteIds;
                }
                else
                {
                    var entities = ItemCheckedModels
                        .Where(m => website.Any(w => int.Parse(w.Replace(",", "")) == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => entities.Any(e => e == b.Website.Name)).ToList();
                    ViewBag.WebsiteId = website;

                }
                website.ForEach(p => ViewBag.website += p + (website.Count > 1 ? "," : ""));
            }

            if (publisher != null && publisher.Count > 0)
            {
                if (publisher[0].IndexOf(",") >= 0)
                {
                    var publisherId = publisher[0].Split(',').ToList();
                    var publishers = ItemCheckedModels
                        .Where(m => publisherId.Any(p => int.Parse(!string.IsNullOrEmpty(p) ? p : "0") == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => publishers.Any(p => p == b.Publisher)).ToList();
                    ViewBag.publisherId = publisherId;
                }

                else
                {
                    var entities = ItemCheckedModels
                        .Where(m => publisher.Any(p => int.Parse(p.Replace(",", "")) == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => entities.Any(p => p == b.Publisher)).ToList();
                    ViewBag.publisherId = publisher;
                }

                publisher.ForEach(p => ViewBag.publisher += p + (publisher.Count > 1 ? "," : ""));
            }

            if (author != null && author.Count > 0)
            {
                if (author[0].IndexOf(",") >= 0)
                {
                    var authorId = author[0].Split(',').ToList();
                    var authors = ItemCheckedModels
                        .Where(m => authorId.Any(a => int.Parse(!string.IsNullOrEmpty(a) ? a : "0") == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => authors.Any(a => a == b.Author)).ToList();
                    ViewBag.authorId = authorId;
                }
                else
                {
                    var entities = ItemCheckedModels
                        .Where(m => author.Any(a => int.Parse(a.Replace(",", "")) == m.ItemId))
                        .GroupBy(m => m.ItemName)
                        .Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => entities.Any(a => a == b.Author)).ToList();
                    ViewBag.authorId = author;
                }

                author.ForEach(a => ViewBag.author += a + (author.Count > 1 ? "," : ""));
            }


            if (!String.IsNullOrEmpty(sort))
            {
                Sort(sort);
            }


            itemViewModel.Books = filteredItems;
            ViewBag.TotalBooks = filteredItems.Count;
            ViewBag.BookPerPage = itemViewModel.BookPerPage;
            ViewBag.CurrentPage = itemViewModel.CurrentPage;
            GetItemsByChecked();

            // checkbox publishers value
            TempData["Publishers"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Publisher");

            // checkbox authors value
            TempData["Authors"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Author");

            // checkbox websites value
            TempData["Websites"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Website");
            if (Request.IsAjaxRequest())
                return PartialView("/Views/Shared/_GetProduct.cshtml", itemViewModel);
            TempData["ItemViewModel"] = itemViewModel;
            return View();

        }

        private List<Book> FilteringByPrice(int? minPrice, int? maxPrice)
        {
            filteredItems = filteredItems.Where(b =>
                double.Parse(b.Price.Trim()) >= minPrice && double.Parse(b.Price.Trim()) <= maxPrice).ToList();
            
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            return filteredItems;
        }

        
        private void Sort(string sortValue)
        {
            foreach (int val in Enum.GetValues(typeof(BookSortModel.Sort)))
            {
                if (int.Parse(sortValue) == val)
                {
                    // val = 1 MinToMax
                    if (int.Parse(sortValue) == 1)
                    {
                        filteredItems = filteredItems.OrderBy(b => ReplaceString(b.Price)).ToList();
                    }
                    // val = 2 MaxToMin
                    else if (int.Parse(sortValue) == 2)
                    {
                        filteredItems = filteredItems.OrderByDescending(b => ReplaceString(b.Price)).ToList();
                    }
                    // val = 3 AToZ
                    else if (int.Parse(sortValue) == 3)
                    {
                        filteredItems = filteredItems.OrderBy(b => b.Name).ToList();
                    }
                    // val = 4 ZToA
                    else if (int.Parse(sortValue) == 4)
                    {
                        filteredItems = filteredItems.OrderByDescending(b => b.Name).ToList();
                    }
                }
            }
        }

        private static void GetItemsByChecked()
        {
            if (ItemCheckedModels.Count == 0)
            {
                var publishers = (books ?? AllBooks).OrderBy(b => b.Publisher).GroupBy(b => b.Publisher)
                    .Select(b => b.Key).Distinct();
                var authors = (books ?? AllBooks).OrderBy(b => b.Author).GroupBy(b => b.Author).Select(b => b.Key)
                    .Distinct();
                var websites = (books ?? AllBooks).OrderBy(b => b.Website.Name).GroupBy(b => b.Website.Name).Select(b => b.Key)
                    .Distinct();
                var itemId = 1;
                foreach (var publisherName in publishers)
                {
                    if (!String.IsNullOrEmpty(publisherName))
                    {
                        var itemCheckedModel = new ItemCheckedModel();
                        itemCheckedModel.ItemEntityName = "Publisher";
                        itemCheckedModel.IsCheck = false;
                        itemCheckedModel.ItemName = publisherName;
                        itemCheckedModel.ItemId = itemId;
                        ItemCheckedModels.Add(itemCheckedModel);
                        itemId++;
                    }

                }

                foreach (var author in authors)
                {
                    if (!String.IsNullOrEmpty(author))
                    {
                        var itemCheckedModel = new ItemCheckedModel();
                        itemCheckedModel.ItemEntityName = "Author";
                        itemCheckedModel.IsCheck = false;
                        itemCheckedModel.ItemName = author;
                        itemCheckedModel.ItemId = itemId;
                        ItemCheckedModels.Add(itemCheckedModel);
                        itemId++;
                    }

                }

                foreach (var website in websites)
                {
                    if (!String.IsNullOrEmpty(website))
                    {
                        var itemCheckedModel = new ItemCheckedModel();
                        itemCheckedModel.ItemEntityName = "Website";
                        itemCheckedModel.IsCheck = false;
                        itemCheckedModel.ItemName = website;
                        itemCheckedModel.ItemId = itemId;
                        ItemCheckedModels.Add(itemCheckedModel);
                        itemId++;
                    }
                }
            }
        }


        public JsonResult Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var distinct = BestSellerBooks.Where(b => b.Name.ToUpper().Trim().Contains(term.ToUpper()))
                    .GroupBy(b => b.Publisher)
                    .Select(b => b.FirstOrDefault())
                    .OrderBy(b => ReplaceString(b.Price)).Distinct().Select(b => new { b.Name, b.Image }).ToList();
                TempData["BooksLogoUrl"] = BooksLogoUrl;


                return Json(distinct, JsonRequestBehavior.AllowGet);
            }

            return Json(BestSellerBooks, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchItem(string query)
        {

            if (!string.IsNullOrEmpty(query))
            {
                var otherPublishers = BestSellerBooks.Where(b => b.Name.ToUpper()
                        .Contains(query.ToUpper()) && b.Publisher != "")
                    .GroupBy(b => new { b.Publisher, b.Name })
                    .Select(b => b.FirstOrDefault())
                    .OrderBy(b => ReplaceString(b.Price)).ToList();


                if (otherPublishers.Count > 1) TempData["OtherPublishers"] = otherPublishers;

                var entities = BestSellerBooks.Where(b => b.Name.Trim() == query.Trim()).Distinct().ToList();
                TempData["SearchText"] = query;
                TempData["Books"] = entities;
                return RedirectToAction("Index", "Home", new RouteValueDictionary(new { query }));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult GetBooksByWebsitePrice(string bookName, string bookPublisher)
        {
            var entities = BestSellerBooks.Where(b => b.Name == bookName && b.Publisher.Contains(bookPublisher))
                .GroupBy(b => b.Website)
                .Select(b => b.FirstOrDefault());
            TempData["Entities"] = entities.ToList();
            TempData["BooksLogoUrl"] = BooksLogoUrl;
            return RedirectToAction("Index");
        }


        public ActionResult GetBestSeller(int page = 1)
        {
            books = new List<Book>(BestSellerBooks);
            TempData["WebsiteBooks"] = books;
            TempData["BooksLogoUrl"] = BooksLogoUrl;
            var booksView = new ItemViewModel
            {
                BookPerPage = 24,
                Books = books,
                CurrentPage = page
            };

            GetItemsByChecked();
            ViewBag.Publishers = ItemCheckedModels.Where(m => m.ItemEntityName == "Publisher");
            ViewBag.Authors = ItemCheckedModels.Where(m => m.ItemEntityName == "Author");
            TempData["Websites"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Website");
            ViewBag.TotalBooks = books.Count;

            return RedirectToAction("GetWebsite", booksView);
        }

        public ActionResult GetAllBooks(int page = 1)
        {
            books = new List<Book>(AllBooks);
            var booksView = new ItemViewModel
            {
                BookPerPage = 24,
                Books = books,
                CurrentPage = page
            };

            GetItemsByChecked();
            ViewBag.Publishers = ItemCheckedModels.Where(m => m.ItemEntityName == "Publisher");
            ViewBag.Authors = ItemCheckedModels.Where(m => m.ItemEntityName == "Author");
            return RedirectToAction("GetWebsite", booksView);
        }

        // Tl ekini kaldırıyor.
        public static float ReplaceString(string price)
        {
            var bookPrice = price.Replace("TL", "");
            return float.Parse(bookPrice);
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

    public class BookNodeXPath
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }

    }


}
