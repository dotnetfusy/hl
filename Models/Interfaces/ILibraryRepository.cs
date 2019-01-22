using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public interface ILibraryRepository
    {
        IEnumerable<LibraryItem> GetAllItems();
        LibraryItem GetItemById(int Id);
        void AddItem(LibraryItem item);
        void EditItem(LibraryItem item);
        void RemoveItem(LibraryItem item);
    }
}
