using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebScraping.Core.Data.Ef;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Data.Concrete.Ef
{
    public class EfBookRepository : EfEntityRepositoryBase<Book, WebScrapingContext>, IBookRepository
    {
        public List<Book> GetBooksWithWebsite()
        {
            using (var context = new WebScrapingContext())
            {
                var books = context.Books.Include(b => b.Website).ToList();
                return books;
            }
        }

        public void DeleteAllRecordsInTable()
        {
            using var context = new WebScrapingContext();
            context.Database.ExecuteSqlCommand("Delete From Books");
        }
    }
}