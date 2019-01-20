using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
        }

        public DbSet<LibraryItem> LibraryItems { get; set; }
    }
}
