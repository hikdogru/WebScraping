using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Business.Abstract;
using WebScraping.Entities;

namespace WebScraping.Business.Concrete
{
    public class BookManager:IBookService
    {
        public List<Book> GetAll()
        {
            throw new NotImplementedException();
        }

        public Book GetById(int bookId)
        {
            throw new NotImplementedException();
        }

        public void Add(Book book)
        {
            throw new NotImplementedException();
        }

        public void Update(Book book)
        {
            throw new NotImplementedException();
        }

        public void Delete(int bookId)
        {
            throw new NotImplementedException();
        }
    }
}
