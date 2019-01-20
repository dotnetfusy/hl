using Biblioteka1.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public class MockLibraryRepository : ILibraryRepository
    {
        private List<LibraryItem> items;

        public MockLibraryRepository()
        {
            if (items == null)
                LoadBooks();
        }

        private void LoadBooks()
        {
            items = new List<LibraryItem>
            {
                new LibraryItem { Title = "Gra o tron", Author = "G.R.R. Martin", Format = "Book", CheckedOut = false, Year = "2014" },
                new LibraryItem { Title = "Same przeboje", Author = "Zenek Martyniuk", Format = "CD", CheckedOut = false, Year = "2015" },
                new LibraryItem { Title = "Breaking Bad Season 1", Author = "Vince Gilligan", Format = "DVD", CheckedOut = false, Year = "2011" }
            };
          }

        public IEnumerable<LibraryItem> GetAllItems()
        {
            return items;
        }

        public LibraryItem GetItemById(int BookId)
        {
            return items.FirstOrDefault(b => b.Id == BookId);
        }

        public void AddItem(LibraryItem item)
        {
            throw new NotImplementedException();
        }

        public void EditItem(LibraryItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(LibraryItem item)
        {
            throw new NotImplementedException();
        }
    }
}
