using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka1.Models.ViewModels
{
    public class StatisticsVM
    {
        public int TotalItems { get; set; }
        public int TotalBook { get; set; }
        public int TotalCD { get; set; }
        public int TotalDVD { get; set; }
        public int TotalCheckedIn { get; set; }
        public int TotalCheckedOut { get; set; }
        public List<LibraryItem> CheckedOutItems { get; set; }

    }
}
