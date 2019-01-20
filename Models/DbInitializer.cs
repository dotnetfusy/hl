using Biblioteka1.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.LibraryItems.Any())
            {
                context.AddRange(
                new LibraryItem { Title = "Gra o tron", Author = "G.R.R. Martin", Format = "Book", CheckedOut = false, Year = "2014" },
                new LibraryItem { Title = "Same przeboje", Author = "Zenek Martyniuk", Format = "CD", CheckedOut = false, Year = "2015" },
                new LibraryItem { Title = "Breaking Bad Season 1", Author = "Vince Gilligan", Format = "DVD", CheckedOut = false, Year = "2011" }
                    );
                context.SaveChanges();
            }

        }

    }
}
