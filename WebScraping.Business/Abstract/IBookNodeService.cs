using System.Collections.Generic;
using WebScraping.Entities;

namespace WebScraping.Business.Abstract
{
    public interface IBookNodeService
    {
        List<BookNode> GetAll();
        BookNode GetById(int bookNodeId);
        void Add(BookNode bookNode);
        void Update(BookNode bookNode);
        void Delete(int bookNodeId);
        List<BookNode> GetNodesByWebsite();


    }
}