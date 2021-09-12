using Bookshelf.Helper;
using Bookshelf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Bookshelf.Controllers
{
    public class HomeController : Controller
    {
        BookshelfApi _api = new BookshelfApi();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<BookData> books = new List<BookData>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/Books/all");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                books = JsonConvert.DeserializeObject<List<BookData>>(results);
            }
            return View(books);
        }

        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult create(BookData book)
        {
            HttpClient client = _api.Initial();

            var postTask = client.PostAsJsonAsync<BookData>("api/Books", book);
            postTask.Wait();

            var result = postTask.Result;

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var book = new BookData();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.DeleteAsync($"api/Books/{Id}");

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details (int Id)
        {
            BookData book = await GetBookByID(Id);
            return View(book);
        }

        private async Task<BookData> GetBookByID(int Id)
        {
            var book = new BookData();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/Books/{Id}");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                book = JsonConvert.DeserializeObject<BookData>(results);
            }

            return book;
        }
        
        public async Task<IActionResult> Edit (int Id)
        {
            BookData book = await GetBookByID(Id);
            return View(book);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(BookData book)
        {
            HttpClient client = _api.Initial();

            var postTask = await client.PutAsJsonAsync($"api/Books/{book.BookId}", book);

            if (postTask.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
     
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
