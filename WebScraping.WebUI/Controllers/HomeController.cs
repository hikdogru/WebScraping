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
        private static readonly List<Book> _books = new List<Book>();
        private static readonly List<Book> _allBooks = new List<Book>();
        private static List<Book> books;
        private static readonly List<ItemCheckedModel> _itemCheckedModels = new List<ItemCheckedModel>();
        private readonly List<string> _bookDetailUrl = new List<string>();

        private readonly Dictionary<string, string> _booksLogoUrl = new Dictionary<string, string>
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
            {"Fidan Kitap", "https://www.fidankitap.com/u/fidankitap/fidan-kitap-logo-9-1576766279.png"},
            {"Hepsiburada", "https://cdn.freelogovectors.net/wp-content/uploads/2018/02/hepsiburada-logo.png"}
        };

        private readonly IBookNodeService _bookNodeService;
        private IWebsiteService _websiteService;
        private readonly IWebsiteUrlService _websiteUrlService;

        private int _time = 0;

        public HomeController(IWebsiteService websiteService, IWebsiteUrlService websiteUrlService,
            IBookNodeService bookNodeService)
        {
            _websiteService = websiteService;
            _websiteUrlService = websiteUrlService;
            _bookNodeService = bookNodeService;
        }


        public ActionResult Index()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            if (_books.Count > 0 == false) Book();

            watch.Stop();
            ViewBag.Time = watch.ElapsedMilliseconds;
            return View(_books);
        }


        private void Book()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var bookNodes = _bookNodeService.GetNodesByWebsite();
            
            foreach (var node in bookNodes)
            {

                if (node.WebsiteId != 2 && node.WebsiteId != 11 && node.Website.Id != 11 && node.Website.Id != 2)
                {

                    foreach (var websiteUrl in node.Website.WebsiteUrls)
                    {
                        var bookDetailNode = SelectNodes(node.Detail, websiteUrl.Url);
                        foreach (var detailNode in bookDetailNode)
                        {

                            _bookDetailUrl.Add((detailNode.GetAttributeValue("href", string.Empty).Contains("http")?"":node.Website.WebsiteUrl) +
                                                   HttpUtility.UrlDecode(
                                                       detailNode.GetAttributeValue("href", string.Empty)));


                        }


                        var bookNodeXpath = new BookNodeXPath()
                        {
                            Name = node.Name,
                            Author = node.Author,
                            Price = node.Price,
                            Image = node.Image,
                            Publisher = node.Publisher,
                        };

                        var bookNode = SelectNodes(bookNodeXpath, websiteUrl.Url);

                        BookScraping(bookNode, node.Website.WebsiteUrl, node.WebsiteId);

                        _bookDetailUrl.Clear();
                    }


                }


                watch.Stop();
                ViewBag.TimeBook = watch.ElapsedMilliseconds;



                //_allBooksUrl = new Dictionary<string, string>();
                //for (int i = 1; i <= 10; i++)
                //{
                //    _allBooksUrl.Add("Bkm Kitap" + i, "https://www.bkmkitap.com/edebiyat-kitaplari?pg=" + i);
                //}

                //foreach (var url in _allBooksUrl)
                //{
                //    if (url.Value.Contains("bkm"))
                //    {
                //        var bookNode = new BookNode()
                //        {
                //            Name = doc.DocumentNode.SelectNodes("//a[@class='fl col-12 text-description detailLink']", url.Value),
                //            Author = doc.DocumentNode.SelectNodes("//a[@class='fl col-12 text-title']", url.Value),
                //            Price = doc.DocumentNode.SelectNodes("//div[@class='col col-12 currentPrice']", url.Value),
                //            Image = doc.DocumentNode.SelectNodes("//span[@class='imgInner']/img", url.Value),
                //            Publisher = doc.DocumentNode.SelectNodes("//a[@class='col col-12 text-title mt']", url.Value),
                //            Detail = doc.DocumentNode.SelectNodes("//a[@class='fl col-12 text-description detailLink']", url.Value),
                //            WebsiteName = "BKM Kitap",
                //            ItemCount = 100
                //        };

                //        foreach (var node in bookNode.Detail)
                //        {
                //            bookDetailUrl.Add("https://www.bkmkitap.com" +
                //                              HttpUtility.UrlDecode(node.GetAttributeValue("href", string.Empty)));
                //        }

                //        BookScraping(bookNode, "https://www.bkmkitap.com", "all");
                //    }

                //}
            }
        }





        private void BookScraping(BookNodeModel bookNode, string websiteUrl, int websiteId, string type = "")
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
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

                if (type == "all")
                    _allBooks.Add(book);
                else
                    _books.Add(book);


                if (i == bookNode.ItemCount) break;
            }
            watch.Stop();
            ViewBag.BookScraping = watch.ElapsedMilliseconds;
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
                watch.Stop();
                _time += Convert.ToInt32(watch.ElapsedMilliseconds);
                TempData["SelectNodes2"] = _time;
                return bookNode;
            }
        }


        public ActionResult GetWebsite(List<string> publisher, List<string> author, int? minPrice = 0,
                int? maxPrice = 0, int? page = 1)
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

            itemViewModel.Books = filteredItems;
            GetItemsByChecked();

            // checkbox publishers value
            TempData["Publishers"] = _itemCheckedModels.Where(m => m.ItemEntityName == "Publisher");

            // checkbox authors value
            TempData["Authors"] = _itemCheckedModels.Where(m => m.ItemEntityName == "Author");

            if (Request.IsAjaxRequest())
                return PartialView("/Views/Shared/_GetProduct.cshtml", itemViewModel);
            return View(itemViewModel);
        }

        private static void GetItemsByChecked()
        {
            if (_itemCheckedModels.Count == 0)
            {
                var publishers = (books ?? _allBooks).OrderBy(b => b.Publisher).GroupBy(b => b.Publisher)
                    .Select(b => b.Key).Distinct();
                var authors = (books ?? _allBooks).OrderBy(b => b.Author).GroupBy(b => b.Author).Select(b => b.Key)
                    .Distinct();
                var itemId = 1;
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


        public ActionResult GetProducts(int page = 1)
        {
            var randomBooks = _books.OrderBy(b => Guid.NewGuid()).Take(20);
            books = new List<Book>(randomBooks);
            TempData["WebsiteBooks"] = books;
            TempData["BooksLogoUrl"] = _booksLogoUrl;
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
                var distinct = _books.Where(b => b.Name.ToUpper().Trim().Contains(term.ToUpper()))
                    .OrderBy(b => ReplaceString(b.Price)).Distinct().Select(b => new { b.Name, b.Image }).ToList();
                TempData["BooksLogoUrl"] = _booksLogoUrl;


                return Json(distinct, JsonRequestBehavior.AllowGet);
            }

            return Json(_books, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchItem(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var otherPublishers = _books.Where(b => b.Name.ToUpper()
                        .Contains(query.ToUpper()) && b.Publisher != "")
                    .GroupBy(b => new { b.Publisher, b.Name })
                    .Select(b => b.FirstOrDefault())
                    .OrderBy(b => ReplaceString(b.Price)).ToList();


                if (otherPublishers.Count > 1) TempData["OtherPublishers"] = otherPublishers;

                var entities = _books.Where(b => b.Name.Trim() == query.Trim()).Distinct().ToList();
                TempData["SearchText"] = query;
                TempData["Books"] = entities;
                return RedirectToAction("Index", "Home", new RouteValueDictionary(new { query }));
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


        public ActionResult GetBestSeller(int page = 1)
        {
            books = new List<Book>(_books);
            TempData["WebsiteBooks"] = books;
            TempData["BooksLogoUrl"] = _booksLogoUrl;
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
            books = new List<Book>(_allBooks);
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
