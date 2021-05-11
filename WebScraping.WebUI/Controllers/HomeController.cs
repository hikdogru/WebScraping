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
        private static readonly List<ItemCheckedModel> _itemCheckedModels = new List<ItemCheckedModel>();
        private readonly List<string> _bookDetailUrl = new List<string>();

        private static readonly Dictionary<string, string> BooksLogoUrl = new Dictionary<string, string>();

        private readonly IBookNodeService _bookNodeService;
        private IWebsiteService _websiteService;
        private readonly IWebsiteUrlService _websiteUrlService;
        private readonly IBookService _bookService;

        private int _time = 0;

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

            //                _bookDetailUrl.Add((detailNode.GetAttributeValue("href", string.Empty).Contains("http") ? "" : node.Website.WebsiteUrl) +
            //                                       HttpUtility.UrlDecode(
            //                                           detailNode.GetAttributeValue("href", string.Empty)));
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


                };

                if (type == "All-books")
                {
                    book.CategoryType = "All-books";
                    _bookService.Add(book);
                }
                else
                {
                    book.CategoryType = "Best-seller";
                    _bookService.Add(book);
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
            IEnumerable<Book> filteredItems = books;
            if (minPrice != 0 && maxPrice != 0)
            {
                filteredItems = filteredItems.Where(b =>
                    double.Parse(b.Price.Trim()) >= minPrice && double.Parse(b.Price.Trim()) <= maxPrice);
                itemViewModel.Books = filteredItems;
                ViewBag.minPrice = minPrice;
                ViewBag.maxPrice = maxPrice;
            }

            if (website != null && website.Count > 0)
            {
                filteredItems = filteredItems.Where(b => website.Any(w => int.Parse(w) == b.WebsiteId));
                website.ForEach(p => ViewBag.website += p + (website.Count > 1 ? "," : ""));

            }

            if (publisher != null && publisher.Count > 0)
            {
                if (publisher[0].IndexOf(",") >= 0)
                {
                    var publisherId = publisher[0].Split(',').ToList();
                    var publishers = _itemCheckedModels
                        .Where(m => publisherId.Any(p => int.Parse(!string.IsNullOrEmpty(p) ? p : "0") == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => publishers.Any(p => p == b.Publisher));
                    ViewBag.publisherId = publisherId;
                }

                else
                {
                    var entities = _itemCheckedModels
                        .Where(m => publisher.Any(p => int.Parse(p.Replace(",", "")) == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
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
                    var authors = _itemCheckedModels
                        .Where(m => authorId.Any(a => int.Parse(!string.IsNullOrEmpty(a) ? a : "0") == m.ItemId))
                        .GroupBy(m => m.ItemName).Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => authors.Any(a => a == b.Author));
                    ViewBag.authorId = authorId;
                }
                else
                {
                    var entities = _itemCheckedModels
                        .Where(m => author.Any(a => int.Parse(a.Replace(",", "")) == m.ItemId))
                        .GroupBy(m => m.ItemName)
                        .Select(m => m.Key);
                    filteredItems = filteredItems.Where(b => entities.Any(a => a == b.Author));
                    ViewBag.authorId = author;
                }

                author.ForEach(a => ViewBag.author += a + (author.Count > 1 ? "," : ""));
            }


            if (!String.IsNullOrEmpty(sort))
            {
                foreach (int val in Enum.GetValues(typeof(BookSortModel.Sort)))
                {
                    if (int.Parse(sort) == val)
                    {
                        // val = 1 MinToMax
                        if (int.Parse(sort) == 1)
                        {
                            filteredItems = filteredItems.OrderBy(b => ReplaceString(b.Price)).ToList();
                        }
                    }
                }

                TempData["Sort"] = sort;

            }
            

            itemViewModel.Books = filteredItems;
            GetItemsByChecked();

            // checkbox publishers value
            TempData["Publishers"] = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");

            // checkbox authors value
            TempData["Authors"] = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");

            if (Request.IsAjaxRequest())
                return PartialView("/Views/Shared/_GetProduct.cshtml", itemViewModel);
            TempData["ItemViewModel"] = itemViewModel;
            return View();

        }

        private static void GetItemsByChecked()
        {
            if (_itemCheckedModels.Count == 0)
            {
                var publishers = (books ?? AllBooks).OrderBy(b => b.Publisher).GroupBy(b => b.Publisher)
                    .Select(b => b.Key).Distinct();
                var authors = (books ?? AllBooks).OrderBy(b => b.Author).GroupBy(b => b.Author).Select(b => b.Key)
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
                        _itemCheckedModels.Add(itemCheckedModel);
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
                        _itemCheckedModels.Add(itemCheckedModel);
                        itemId++;
                    }

                }
            }
        }

        //Carousel
        public ActionResult GetProducts(int page = 1)
        {
            var randomBooks = BestSellerBooks.OrderBy(b => Guid.NewGuid()).Take(20);
            books = new List<Book>(randomBooks);
            TempData["WebsiteBooks"] = books;
            TempData["BooksLogoUrl"] = BooksLogoUrl;
            var booksView = new ItemViewModel
            {
                BookPerPage = 24,
                Books = books,
                CurrentPage = page
            };

            GetItemsByChecked();
            ViewBag.Publishers = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");
            ViewBag.Authors = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");

            return RedirectToAction("GetWebsite", booksView);
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
            ViewBag.Publishers = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");
            ViewBag.Authors = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");

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
            ViewBag.Publishers = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");
            ViewBag.Authors = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");
            return RedirectToAction("GetWebsite", booksView);
        }

        // Tl ekini kaldırıyor.
        public static int ReplaceString(string price)
        {
            var bookPrice = price.Replace("TL", "");
            return (int)float.Parse(bookPrice);
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
