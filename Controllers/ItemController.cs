using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteka1.Models;
using Biblioteka1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using System.Diagnostics;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Biblioteka1.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private IHostingEnvironment _env;
        private IConfiguration _configuration;

        public int PageSize = 5;

        public ItemController(ILibraryRepository libraryRepository, IHostingEnvironment env, IConfiguration Configuration)
        {
            _libraryRepository = libraryRepository;
            _env = env;
            _configuration = Configuration;
        }

        // GET: /<controller>/
        public IActionResult Index(string searchTitle=null, string searchAuthor = null, string searchFormat = null, int page = 1)
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

            var itemVM = new ListVM()
            {
                LibraryItems = items.OrderBy(i => i.Title).Skip((page - 1) * PageSize).Take(PageSize).OrderBy(i => i.Title).ToList(),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = _libraryRepository.GetAllItems().Count()
                    
                }
            };

            return View(itemVM);
        }

        public IActionResult Details (int id)
        {
            var item = _libraryRepository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (LibraryItem item)
        {
            bool uploadSuccess;

            IFormFile file = HttpContext.Request.Form.Files.FirstOrDefault();

            using (var stream = file.OpenReadStream())
            {
                uploadSuccess = await UploadToBlob(file.FileName, item, null, stream);
            }

            if (ModelState.IsValid)
            {
                _libraryRepository.AddItem(item);
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var item = _libraryRepository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit (LibraryItem item)
        {
            bool uploadSuccess;

            IFormFile file = HttpContext.Request.Form.Files.FirstOrDefault();

            using (var stream = file.OpenReadStream())
            {
                uploadSuccess = await UploadToBlob(file.FileName, item, null, stream);
            }

            if (ModelState.IsValid)
            {
                _libraryRepository.EditItem(item);
            }
            else
            {
                return View(item);  
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<bool> UploadToBlob(string filename, LibraryItem item, byte[] imageBuffer = null, Stream stream = null)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = "UseDevelopmentStorage=true;";

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("uploadblob" + Guid.NewGuid().ToString());
                    await cloudBlobContainer.CreateAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        // OPTION A: use imageBuffer (converted from memory stream)
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
                        // OPTION B: pass in memory stream directly
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return false;
                    }

                    string address = cloudBlockBlob.Uri.ToString();
                    item.CoverString = address;

                    return true;
                }
                catch (StorageException ex)
                {
                    return false;
                }
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                }
            }
            else
            {
                return false;
            }

        }


        public IActionResult Delete(int id)
        {
            var item = _libraryRepository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _libraryRepository.GetItemById(id);

                _libraryRepository.RemoveItem(item);
                return RedirectToAction(nameof(Index));
        }

        public IActionResult CheckOut(int id)
        {
            var item = _libraryRepository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut(LibraryItem item, [FromServices] UserManager<IdentityUser> _userManager)
        {
            if (ModelState.IsValid)
            {

                if (item.CheckedOutTo == null)
                {
                    ModelState.AddModelError(string.Empty, "CHecked out to name must be added.");
                    return View(item);
                }
                item.CheckedOut = true;
                item.CheckedOutBy = _userManager.GetUserName(HttpContext.User);
                item.CheckedOutDate = DateTime.Today.ToString(); 

                _libraryRepository.EditItem(item);

                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        public IActionResult CheckIn(int id)
        {
            var item = _libraryRepository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn(LibraryItem item)
        {
            if (ModelState.IsValid)
            {
                item.CheckedOut = false;
                item.CheckedOutBy = null;
                item.CheckedOutTo = null;
                item.CheckedOutDate = null;

                _libraryRepository.EditItem(item);

                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

    }
}
