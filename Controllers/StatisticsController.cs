using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteka1.Models;
using Biblioteka1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteka1.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;

        public StatisticsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var items = _libraryRepository.GetAllItems();
            var statisticsVM = new StatisticsVM()
            {
                TotalItems = items.Count(),
                TotalBook = items.Where(i => i.Format == "Book").Count(),
                TotalCD = items.Where(i => i.Format == "CD").Count(),
                TotalDVD = items.Where(i => i.Format == "DVD").Count(),
                TotalCheckedIn = items.Where(i=>i.CheckedOut==false).Count(),
                TotalCheckedOut = items.Where(i => i.CheckedOut == true).Count(),
                CheckedOutItems= items.Where(i => i.CheckedOut == true).ToList()
            };

            return View(statisticsVM);
        }
    }
}