using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Entities;

namespace WebScraping.Business.Abstract
{
    public interface IBookService
    {
        List<Book> GetAll();
        Book GetById(int bookId);
        bool Add(Book book);
        void Update(Book book);
        void Delete(int bookId);
        List<Book> GetBooksWithWebsite();
    }
}
