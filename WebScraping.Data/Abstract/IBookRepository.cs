using System.Collections.Generic;
using WebScraping.Core.Data;
using WebScraping.Entities;

namespace WebScraping.Data.Abstract
{
    public interface IBookRepository:IEntityRepository<Book>
    {
        List<Book> GetBooksWithWebsite();

        void DeleteAllRecordsInTable();
    }
}