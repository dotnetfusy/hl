using Biblioteka1.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models.ViewModels
{
    public class ListVM
    {
        public string SiteTitle { get; set; }
        public List<LibraryItem> LibraryItems { get; set; }
        public string SearchTitle { get; set; }
        public string SearchAuthor { get; set; }
        public string SearchFormat { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
