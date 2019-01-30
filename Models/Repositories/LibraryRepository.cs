using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly AppDbContext _appDbContext;

        public LibraryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public IEnumerable<LibraryItem> GetAllItems()
        {
            return _appDbContext.LibraryItems;
        }

        public LibraryItem GetItemById(int ItemId)
        {
            return _appDbContext.LibraryItems.FirstOrDefault(i => i.Id == ItemId);
        }

        public void AddItem(LibraryItem item)
        {
            _appDbContext.LibraryItems.Add(item);
            _appDbContext.SaveChanges();
        }

        public void EditItem(LibraryItem item)
        {
            _appDbContext.LibraryItems.Update(item);
            _appDbContext.SaveChanges();
        }

        public void RemoveItem(LibraryItem item)
        {
            _appDbContext.LibraryItems.Remove(item);
            _appDbContext.SaveChanges();
        }

    }
}
