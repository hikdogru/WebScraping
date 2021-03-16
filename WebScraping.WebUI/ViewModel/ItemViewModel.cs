﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebScraping.WebUI.Models;

namespace WebScraping.WebUI.ViewModel
{
    public class ItemViewModel
    {
        public IEnumerable<BookModel> Books { get; set; }
        public int BookPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(Books.Count() / (double)BookPerPage));
        }
        public IEnumerable<BookModel> PaginatedBooks()
        {
            int start = (CurrentPage - 1) * BookPerPage;
            return Books.OrderBy(b => b.Id).Skip(start).Take(BookPerPage);
        }
    }
}