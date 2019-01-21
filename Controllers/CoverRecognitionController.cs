﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Threading;
using System.Text;
using Biblioteka1.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Biblioteka1.Controllers
{
    public class CoverRecognitionController : Controller
    {
        List<string> result = new List<string>(); 
        string ImagePath;

        public IActionResult Index()
        {
            return View();
        }

       
        private IHostingEnvironment _environment;

        public CoverRecognitionController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        
        [HttpPost]
        public async Task<IActionResult> Index(ICollection<IFormFile> files)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
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
            ViewBag.Resoult = result; 

            return View();
        }
        
        public IActionResult CoverRecognition()
        {            
            return View();
        }
             
                
         async Task MakeRequest() 
        {
            
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
                                    
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "01afb524d5da4b7eb2fac5ddd3215799"); //klucz ważny do 26.01.2019 
            
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
                        foreach (Word _word in _line.Words)
                        { result.Add(_word.Text); }                        
                    }                                     
                    
                }                
            }
            
        }               
    }
}