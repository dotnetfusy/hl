using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Biblioteka1.Models;
using Biblioteka1.Models.ViewModels;
using Biblioteka1.Models.Enumerations;

namespace Biblioteka1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        public int PageSize = 5;

        public HomeController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public IActionResult Index(string searchTitle, string searchAuthor, string searchFormat, int page = 1)
        {
            var items = _libraryRepository.GetAllItems();

            if (searchTitle != null)
            {
                items = items.Where(i => i.Title.ToLower().Contains(searchTitle.ToLower()));
            };
            if (searchAuthor != null)
            {
                items = items.Where(i => i.Author.ToLower().Contains(searchAuthor.ToLower()));
            };
            if (searchFormat != null)
            {
                items = items.Where(i => i.Format.ToLower().Contains(searchFormat.ToLower()));
            };

            var homeVM = new ListVM()

            {

                SiteTitle = "The list of books",
                LibraryItems = items.OrderBy(i => i.Title).Skip((page-1)*PageSize).Take(PageSize).ToList(),
                SearchTitle = searchTitle,
                SearchAuthor = searchAuthor,
                SearchFormat = searchFormat,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = items.Count()
                }
            };

            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            var item = _libraryRepository.GetItemById(id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

    }
}
