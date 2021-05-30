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
    public class BookManager : IBookService
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
            //var books = _bookRepository.GetList().Where(b=>b.BookDetailUrl == book.BookDetailUrl && b.CategoryType == book.CategoryType);

            //if (books.Count()<1)
            //{
            //}

            _bookRepository.Add(book);





        }

        public void Update(Book book)
        {
            _bookRepository.Update(book);
        }

        public void Delete(int bookId)
        {
            _bookRepository.Delete(new Book() { Id = bookId });

        }

        public List<Book> GetBooksWithWebsite()
        {
            return _bookRepository.GetBooksWithWebsite();
        }
    }
}
