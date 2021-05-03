using System;
using System.Collections.Generic;
using System.Linq;
using WebScraping.Entities;

namespace WebScraping.WebUI.ViewModel
{
    public class Pager
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<Book> Books { get; set; }


        public (int startPage, int endPage) GetPageNumber()
        {
            var startPage = CurrentPage - 5;
            var endPage = CurrentPage + 4;
            if (startPage <= 0)
            {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if (endPage > PageCount())
            {
                endPage = PageCount();
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            return (startPage, endPage);
        }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(Books.Count() / (double)PageSize));
        }

        public IEnumerable<Book> PaginatedBooks()
        {
            int start = (CurrentPage - 1) * PageSize;
            return Books.Skip(start).Take(PageSize);
        }

        


    }
}