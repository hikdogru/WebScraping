﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using WebScraping.Business.Abstract;
using WebScraping.Entities;
using WebScraping.WebUI.Models;
using WebScraping.WebUI.ViewModel;


namespace WebScraping.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private static List<Book> _bestSellerBooks = new();
        private static List<Book> _allBooks = new();
        private static List<Book> books;
        private static List<Book> _tempAllBooks = new();
        private static readonly List<ItemCheckedModel> ItemCheckedModels = new List<ItemCheckedModel>();
        private List<Book> _filteredItems;
        private static List<Book> _allBookList = new List<Book>();
        


        private static readonly Dictionary<string, string> BooksLogoUrl = new Dictionary<string, string>();


        private readonly IBookNodeService _bookNodeService;
        private readonly IWebsiteService _websiteService;
        private readonly IBookService _bookService;


        public HomeController(IWebsiteService websiteService,
            IBookNodeService bookNodeService, IBookService bookService)
        {
            _websiteService = websiteService;
            _bookNodeService = bookNodeService;
            _bookService = bookService;

        }


        public ActionResult Index()
        {
            if (_allBookList.Count == 0) Book();
            return View(_allBookList);
        }


        public void CallBookMethod(object sender, ElapsedEventArgs e)
        {
            Scrape();
        }


        private void Book()
        {
            var websites = _websiteService.GetAll();
            foreach (var website in websites)
            {
                // for dictionary same key error
                if (!BooksLogoUrl.ContainsKey(website.Name))
                {
                    BooksLogoUrl.Add(website.Name, website.LogoUrl);
                }
            }

            if (_bookService.GetBooksWithWebsite().Count == 0)
            {
                Scrape();
            }

            var bestSellerBooks = _bookService.GetBooksWithWebsite().Where(b => b.CategoryType == "Best-Seller");
            _bestSellerBooks = new List<Book>(bestSellerBooks);

            var allBooks = _bookService.GetBooksWithWebsite().Where(b => b.CategoryType == "All-books");
            _allBookList = new List<Book>(allBooks);
        }

        private void Scrape()
        {
            var bookNodes = _bookNodeService.GetNodesByWebsite();
            Parallel.ForEach(bookNodes, node =>
               {
                   {
                       if (node.WebsiteId != 2 && node.WebsiteId != 11 && node.Website.Id != 11 && node.Website.Id != 2)
                       {
                           foreach (var websiteUrl in node.Website.WebsiteUrls)
                           {
                               var bookNodeXpath = new BookNodeXPath()
                               {
                                   Name = node.Name,
                                   Author = node.Author,
                                   Price = node.Price,
                                   Image = node.Image,
                                   Publisher = node.Publisher,
                                   Detail = node.Detail
                               };

                               var bookNode = SelectNodes(bookNodeXpath, websiteUrl);
                               foreach (var i in bookNode)
                                   BookScraping(i, node.Website.WebsiteUrl, node.WebsiteId, websiteUrl.UrlType);
                           };
                       }
                   }
               });
            _bookService.DeleteAllRecordsInTable();
            _tempAllBooks.ForEach(b => _bookService.Add(b));
        }

        private void BookScraping(BookNodeModel bookNode, string websiteUrl, int websiteId, string type = "")
        {
            string bookPrice = "";
            for (var i = 0; i < bookNode.Author.Count; i++)
            {
                var bookName = WebUtility
                    .HtmlDecode(bookNode.Name == null ? "" : bookNode.Name[i].InnerText.Replace("\n", "")).Trim();
                var author = WebUtility.HtmlDecode(bookNode.Author[i].InnerText.Replace("\n", "")).Trim();
                var publisher = WebUtility.HtmlDecode(bookNode.Publisher?[i].InnerText);
                string image = "";
                if (bookNode.Image != null)
                    image = WebUtility.HtmlDecode(bookNode.Image[i].GetAttributeValue("data-src", "") != ""
                        ? bookNode.Image[i].GetAttributeValue("data-src", "")
                        : bookNode.Image[i].GetAttributeValue("src", ""));

                if (websiteUrl.Contains("kidega")) image = WebUtility.HtmlDecode(bookNode.Image[i].GetAttributeValue("data-original", ""));

                string bookDetail = (bookNode.Detail[i].GetAttributeValue("href", string.Empty).Contains("http")
                                        ? ""
                                        : websiteUrl) +
                                    HttpUtility.UrlDecode(
                                        bookNode.Detail[i].GetAttributeValue("href", string.Empty));

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

                var book = new Book
                {
                    Id = i + 1,
                    Name = bookName,
                    Author = author,
                    Price = bookPrice,
                    Publisher = publisher,
                    WebsiteId = websiteId,
                    Image = image,
                    BookDetailUrl = bookDetail,
                    CategoryType = type
                };

                _tempAllBooks.Add(book);
            }
        }




        private List<BookNodeModel> SelectNodes(BookNodeXPath bookNodeXPath, WebsiteUrl websiteUrl)
        {
            var bookNodeModelList = new List<BookNodeModel>();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using var myWebClient = new MyWebClient();
            myWebClient.Headers["User-Agent"] =
                "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            myWebClient.Headers["Accept"] =
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

            var page = myWebClient.DownloadData(websiteUrl.Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(Encoding.UTF8.GetString(page));

            var nameNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Name);
            var authorNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Author);
            var publisherNode = String.IsNullOrEmpty(bookNodeXPath.Publisher) ? null : doc.DocumentNode.SelectNodes(bookNodeXPath.Publisher);
            var imageNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Image);
            var priceNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Price);
            var detailNode = doc.DocumentNode.SelectNodes(bookNodeXPath.Detail);
            var bookNode = new BookNodeModel
            {
                Name = nameNode,
                Author = authorNode,
                Price = priceNode,
                Image = imageNode,
                Publisher = publisherNode,
                Detail = detailNode,
                ItemCount = 200,

            };
            bookNodeModelList.Add(bookNode);
            return bookNodeModelList;
        }




        public ActionResult GetWebsite(List<string> website, List<string> publisher, List<string> author, int? minPrice = 0,
                int? maxPrice = 0, int? page = 1, string sort = "")
        {
            var itemViewModel = new ItemViewModel();
            itemViewModel.BookPerPage = 24;
            itemViewModel.CurrentPage = (int)page;
            _filteredItems = books;
            if (minPrice != 0 && maxPrice != 0)
                itemViewModel.Books = FilteringByPrice(minPrice, maxPrice);

            if (website != null && website.Count > 0)
                Filtering(website, "Website");

            if (publisher != null && publisher.Count > 0)
                Filtering(publisher, "Publisher");

            if (author != null && author.Count > 0)
                Filtering(author, "Author");

            if (!String.IsNullOrEmpty(sort))
                Sort(sort);

            itemViewModel.Books = _filteredItems;
            ViewBag.TotalBooks = _filteredItems.Count;
            ViewBag.BookPerPage = itemViewModel.BookPerPage;
            ViewBag.CurrentPage = itemViewModel.CurrentPage;


            //GetItemsByChecked();
            // checkbox publishers value
            TempData["Publishers"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Publisher");

            // checkbox authors value
            TempData["Authors"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Author");

            // checkbox websites value
            TempData["Websites"] = ItemCheckedModels.Where(m => m.ItemEntityName == "Website");

            TempData["ItemViewModel"] = itemViewModel;
            TempData["SliderBooks"] = books;

            if (Request.IsAjaxRequest())
                return PartialView("/Views/Shared/_GetProduct.cshtml", itemViewModel);
            return View();

        }


        /// <summary>
        ///  Book filtering method by websitename, author and publisher
        /// </summary>
        /// <param name="entityIdList">The id list of the elements of the filtered class</param>
        /// <param name="entityName">Filtered class name</param>
        private void Filtering(List<string> entityIdList, object entityName)
        {
            IEnumerable<string> entities = null;
            List<string> idList = new();
            if (entityIdList[0].IndexOf(",") >= 0)
            {
                idList = entityIdList[0].Split(',').ToList();
                entities = ItemCheckedModels
                    .Where(m => idList.Any(a => int.Parse(!string.IsNullOrEmpty(a) ? a : "0") == m.ItemId))
                    .GroupBy(m => m.ItemName).Select(m => m.Key);
            }
            else
            {
                idList = entityIdList.Distinct().ToList();
                entities = ItemCheckedModels
                        .Where(m => entityIdList.Any(a => int.Parse(a.Replace(",", "")) == m.ItemId))
                        .GroupBy(m => m.ItemName)
                        .Select(m => m.Key);
            }

            if (entityName == "Author")
            {
                _filteredItems = _filteredItems.Where(b => entities.Any(a => a == b.Author)).ToList();
                ViewBag.authorId = idList;
            }

            else if (entityName == "Publisher")
            {
                _filteredItems = _filteredItems.Where(b => entities.Any(a => a == b.Publisher)).ToList();
                ViewBag.publisherId = idList;
            }
            else if (entityName == "Website")
            {
                _filteredItems = _filteredItems.Where(b => entities.Any(a => a == b.Website.Name)).ToList();
                ViewBag.WebsiteId = idList;
            }
        }

        private List<Book> FilteringByPrice(int? minPrice, int? maxPrice)
        {
            _filteredItems = _filteredItems.Where(b =>
                double.Parse(b.Price.Trim()) >= minPrice && double.Parse(b.Price.Trim()) <= maxPrice).ToList();

            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            return _filteredItems;
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
                        _filteredItems = _filteredItems.OrderBy(b => ReplaceString(b.Price)).ToList();
                    }
                    // val = 2 MaxToMin
                    else if (int.Parse(sortValue) == 2)
                    {
                        _filteredItems = _filteredItems.OrderByDescending(b => ReplaceString(b.Price)).ToList();
                    }
                    // val = 3 AToZ
                    else if (int.Parse(sortValue) == 3)
                    {
                        _filteredItems = _filteredItems.OrderBy(b => b.Name).ToList();
                    }
                    // val = 4 ZToA
                    else if (int.Parse(sortValue) == 4)
                    {
                        _filteredItems = _filteredItems.OrderByDescending(b => b.Name).ToList();
                    }
                }
            }
        }

        private static void GetItemsByChecked(string categoryType = "")
        {
            if (ItemCheckedModels.Count == 0 || !String.IsNullOrEmpty(categoryType))
            {
                ItemCheckedModels.Clear();
                var publishers = (books ?? _allBooks).OrderBy(b => b.Publisher).GroupBy(b => b.Publisher)
                    .Select(b => b.Key).Distinct();
                var authors = (books ?? _allBooks).OrderBy(b => b.Author).GroupBy(b => b.Author).Select(b => b.Key)
                    .Distinct();
                var websites = (books ?? _allBooks).OrderBy(b => b.Website.Name).GroupBy(b => b.Website.Name)
                    .Select(b => b.Key)
                    .Distinct();
                var itemId = 1;
                foreach (var publisherName in publishers)
                {
                    if (!String.IsNullOrEmpty(publisherName))
                    {
                        ItemCheckedModel itemCheckedModel = new();
                        CreateItemCheckedModel(itemCheckedModel, "Publisher", publisherName, categoryType, itemId);
                        itemId++;
                    }
                }
                foreach (var author in authors)
                {
                    if (!String.IsNullOrEmpty(author))
                    {
                        ItemCheckedModel itemCheckedModel = new();
                        CreateItemCheckedModel(itemCheckedModel, "Author", author, categoryType, itemId);
                        itemId++;
                    }
                }
                foreach (var website in websites)
                {
                    if (!String.IsNullOrEmpty(website))
                    {
                        ItemCheckedModel itemCheckedModel = new();
                        CreateItemCheckedModel(itemCheckedModel, "Website", website, categoryType, itemId);
                        itemId++;
                    }
                }
            }
        }

        private static void CreateItemCheckedModel(ItemCheckedModel itemCheckedModel, string entityName, string entityValue, string categoryType, int itemId)
        {
            itemCheckedModel.ItemEntityName = entityName;
            itemCheckedModel.IsCheck = false;
            itemCheckedModel.ItemName = entityValue;
            itemCheckedModel.ItemCategoryType = categoryType;
            itemCheckedModel.ItemId = itemId;
            ItemCheckedModels.Add(itemCheckedModel);
        }


        // Search by autocomplete
        public JsonResult Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var distinct = _allBookList.Where(b => b.Name.ToUpper().Trim().Contains(term.ToUpper()) || b.Author.ToUpper().Trim().Contains(term.ToUpper()))
                    .GroupBy(b => b.Publisher)
                    .Select(b => b.FirstOrDefault())
                    .OrderBy(b => ReplaceString(b.Price)).Distinct().Select(b => new { b.Name, b.Image }).ToList();
                TempData["BooksLogoUrl"] = BooksLogoUrl;


                return Json(distinct, JsonRequestBehavior.AllowGet);
            }

            return Json(_allBookList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SearchItem(string query, string searchOptionSelect)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var otherPublishers = _allBookList.Where(b => b.Name.ToUpper()
                        .Contains(query.ToUpper()) && b.Publisher != "")
                    .Distinct()
                    .GroupBy(b => new { b.Publisher })
                    .Select(b => b.FirstOrDefault())
                    .OrderBy(b => ReplaceString(b.Price)).ToList();

                if (!String.IsNullOrEmpty(searchOptionSelect)) otherPublishers = otherPublishers.Where(b => b.CategoryType == searchOptionSelect).ToList();

                if (otherPublishers.Count > 1) TempData["OtherPublishers"] = otherPublishers;

                var entities = _allBookList.Where(b => b.Name.ToUpper().Trim().Contains(query.ToUpper().Trim())).Distinct().GroupBy(b => b.Website).Select(b => b.FirstOrDefault()).OrderBy(b => ReplaceString(b.Price)).ToList();
                if (!String.IsNullOrEmpty(searchOptionSelect)) entities = entities.Where(b => b.CategoryType == searchOptionSelect).ToList();

                TempData["SearchText"] = query;
                TempData["Books"] = entities;
            }
            TempData["BooksLogoUrl"] = BooksLogoUrl;

            return View("../Product/Search");
        }

        [HttpPost]
        public ActionResult GetBooksByWebsitePrice(string bookName, string bookPublisher)
        {
            var entities = _allBookList.Where(b => b.Name == bookName && (b.Publisher == null ? false : b.Publisher.Contains(bookPublisher)))
                .GroupBy(b => b.Website)
                .Select(b => b.FirstOrDefault())
                .OrderBy(b => ReplaceString(b?.Price));
            TempData["Entities"] = entities.ToList();
            TempData["BooksLogoUrl"] = BooksLogoUrl;
            return View("../Product/Search");
        }


        public ActionResult GetBestSeller(int page = 1)
        {
            var bestSellerBooks = _bookService.GetBooksWithWebsite().Where(b => b.CategoryType == "Best-Seller").OrderBy(b => b.Id);
            books = new List<Book>(bestSellerBooks);
            TempData["WebsiteBooks"] = books;
            TempData["BooksLogoUrl"] = BooksLogoUrl;
            var booksView = new ItemViewModel
            {
                BookPerPage = 24,
                Books = books,
                CurrentPage = page
            };

            GetItemsByChecked("Best-Seller");
            GetCheckboxValues("Best-Seller");
            ViewBag.TotalBooks = books.Count;
            return RedirectToAction("GetWebsite", booksView);
        }

        private void GetCheckboxValues(string categoryType)
        {
            // checkbox publishers value
            TempData["Publishers"] =
                ItemCheckedModels.Where(m => m.ItemEntityName == "Publisher" && m.ItemCategoryType == categoryType);
            // checkbox authors value
            TempData["Authors"] =
                ItemCheckedModels.Where(m => m.ItemEntityName == "Author" && m.ItemCategoryType == categoryType);
            // checkbox websites value
            TempData["Websites"] =
                ItemCheckedModels.Where(m => m.ItemEntityName == "Website" && m.ItemCategoryType == categoryType);
        }

        public ActionResult GetAllBooks(int page = 1)
        {
            var allBooks = _bookService.GetBooksWithWebsite().Where(b => b.CategoryType == "All-books");
            books = new List<Book>(allBooks);
            var booksView = new ItemViewModel
            {
                BookPerPage = 24,
                Books = books,
                CurrentPage = page
            };
            GetItemsByChecked("All-books");
            GetCheckboxValues("All-books");
            ViewBag.TotalBooks = books.Count;
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
}

