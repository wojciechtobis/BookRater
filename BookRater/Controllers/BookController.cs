using Microsoft.AspNetCore.Mvc;

namespace BookRater.Controllers
{
    using System;
    using System.Threading.Tasks;
    using BookRater.Services;
    using Microsoft.AspNetCore.Mvc;
    using BookRater.Models;

    public class BookController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public BookController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _cosmosDbService.GetItemsAsync("SELECT * FROM c"));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,Title,Author")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.Id = Guid.NewGuid().ToString();
                await _cosmosDbService.AddItemAsync(book);
                return RedirectToAction("Index");
            }

            return View(book);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,Title,Author")] Book book)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateItemAsync(book.Id, book);
                return RedirectToAction("Index");
            }

            return View(book);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Book item = await _cosmosDbService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Book book = await _cosmosDbService.GetItemAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetItemAsync(id));
        }
    }
}
