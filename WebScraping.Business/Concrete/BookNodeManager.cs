using System.Collections.Generic;
using WebScraping.Business.Abstract;
using WebScraping.Data.Abstract;
using WebScraping.Entities;

namespace WebScraping.Business.Concrete
{
    public class BookNodeManager:IBookNodeService
    {
        private IBookNodeRepository _bookNodeRepository;

        public BookNodeManager(IBookNodeRepository bookNodeRepository)
        {
            _bookNodeRepository = bookNodeRepository;
        }

        public List<BookNode> GetAll()
        {
            return _bookNodeRepository.GetList();
        }

        public BookNode GetById(int bookNodeId)
        {
            return _bookNodeRepository.Get(b => b.Id == bookNodeId);
        }

        public void Add(BookNode bookNode)
        {
            _bookNodeRepository.Add(bookNode);
        }

        public void Update(BookNode bookNode)
        {
            _bookNodeRepository.Update(bookNode);
        }

        public void Delete(int bookNodeId)
        {

            var bookNode = _bookNodeRepository.Get(b => b.Id == bookNodeId);
            if (bookNode != null)
            {
                _bookNodeRepository.Delete(bookNode);
            }
        }

        public List<BookNode> GetNodesByWebsite()
        {
            return _bookNodeRepository.GetNodesByWebsite();
        }
    }
}