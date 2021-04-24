using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraping.Business.Abstract;
using WebScraping.Data.Abstract;
using WebScraping.Data.Concrete.Ef;
using WebScraping.Entities;


namespace WebScraping.Business.Concrete
{
    public class BookManager:IBookService
    {
        private IBookRepository _bookRepository;

        public BookManager(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }



        public List<Book> GetAll()
        {
            return _bookRepository.GetList();
        }

        public Book GetById(int bookId)
        {
            return _bookRepository.Get(b => b.Id == bookId);
        }

        public void Add(Book book)
        {
            _bookRepository.Add(book);
        }

        public void Update(Book book)
        {
            _bookRepository.Update(book);
        }

        public void Delete(int bookId)
        {
            var book = _bookRepository.Get(b => b.Id == bookId);
            if (book != null)
            {
                _bookRepository.Delete(book);
            }
        }
    }
}
