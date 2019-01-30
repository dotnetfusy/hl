using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biblioteka1.Models;
using Biblioteka1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Threading;


namespace Biblioteka1.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private IHostingEnvironment _env;
        private IConfiguration _configuration;
        private string ImagePath;

        List<string> result = new List<string>();


        public int PageSize = 5;

        public ItemController(ILibraryRepository libraryRepository, IHostingEnvironment env, IConfiguration Configuration)
        {
            _libraryRepository = libraryRepository;
            _env = env;
            _configuration = Configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CoverRecognition(ICollection<IFormFile> files)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            IFormFile xfile = HttpContext.Request.Form.Files.FirstOrDefault();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    ImagePath = Path.Combine(uploads, xfile.FileName);
                    using (var fileStream = new FileStream(ImagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }

            MakeRequest().GetAwaiter().GetResult();
            ViewBag.Result = result;

            return View("Create");
            // return PartialView("CoverRecognition");
        }

        public IActionResult CoverRecognition()
        {
            return PartialView("CoverRecognition");
        }

        // GET: /<controller>/
        public IActionResult Index(string searchTitle = null, string searchAuthor = null, string searchFormat = null, int page = 1)
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

        public IActionResult Details(int id)
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
        public async Task<IActionResult> Create(LibraryItem item)
        {
            bool uploadSuccess;

            IFormFile file = HttpContext.Request.Form.Files.FirstOrDefault();

            if (file != null)
            {
                using (var stream = file.OpenReadStream())
                {
                    uploadSuccess = await UploadToBlob(file.FileName, item, null, stream);
                }
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
        public async Task<IActionResult> Edit(LibraryItem item)
        {
            bool uploadSuccess;

            IFormFile file = HttpContext.Request.Form.Files.FirstOrDefault();

            if (file != null)
            {
                using (var stream = file.OpenReadStream())
                {
                    uploadSuccess = await UploadToBlob(file.FileName, item, null, stream);
                }

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
        public IActionResult CheckOut(int id, string checkedOutTo, [FromServices] UserManager<IdentityUser> _userManager)
        {
            var item = _libraryRepository.GetItemById(id);

            if (item != null)
            {

                item.CheckedOutTo = checkedOutTo;
                item.CheckedOut = true;
                item.CheckedOutBy = _userManager.GetUserName(HttpContext.User);
                item.CheckedOutDate = DateTime.Today.ToString();

                if (item.CheckedOutTo == null)
                {
                    ModelState.AddModelError(string.Empty, "Checked out to name must be added.");
                    return View(item);
                }

                _libraryRepository.EditItem(item);

                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpGet]
        public IActionResult CheckIn(int id)
        {
            var item = _libraryRepository.GetItemById(id);
            if (item == null)
                return NotFound();

            return View(item);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn(int id, bool CheckedOut)
        {
            var item = _libraryRepository.GetItemById(id);

            if (item != null)
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

        public async Task<bool> UploadToBlob(string filename, LibraryItem item, byte[] imageBuffer = null, Stream stream = null)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = "UseDevelopmentStorage=true;";

            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("uploadblob" + Guid.NewGuid().ToString());
                    await cloudBlobContainer.CreateAsync();

                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

                    if (imageBuffer != null)
                    {
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                    }
                    else if (stream != null)
                    {
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
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task MakeRequest()
        {

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "18daa975a1de4b65b7a7204a5248b81f"); //valid 03/02/2019

            queryString["mode"] = "Printed";
            var uri = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/recognizeText?" + queryString;

            HttpResponseMessage response;
            byte[] byteData = GetImageAsByteArray(ImagePath);
            byte[] GetImageAsByteArray(string imageFilePath)
            {
                using (FileStream fileStream =
                    new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    return binaryReader.ReadBytes((int)fileStream.Length);
                }
            }

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                IEnumerable<string> str = response.Headers.GetValues("Operation-Location");

                foreach (var item in str)
                {
                    string responseContent;
                    do
                    {
                        HttpResponseMessage message = await client.GetAsync(item);
                        responseContent = await message.Content.ReadAsStringAsync();
                        Thread.Sleep(500);
                    }
                    while (responseContent == "{\"status\":\"Running\"}");
                    Rootobject @object = Newtonsoft.Json.JsonConvert.DeserializeObject<Rootobject>(responseContent);

                    foreach (Line _line in @object.RecognitionResult.Lines)
                    {
                        { result.Add(_line.Text); }
                    }

                }
            }
        }
    }
}
