using Common.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class BooksController : Controller
    {
        private BooksRepository _bookRepository;
        public BooksController(BooksRepository booksRepository) { 
        _bookRepository = booksRepository;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet] //this is going to load a page with empty textboxes which the user can fill with data
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] //this is going to be called when the user hits the submit button - this is saving the data into the db
        public IActionResult Create(BookViewModel b)
        {
            ModelState.Remove("Id");
         //   ModelState.Remove("Category");
            ModelState.Remove("Filename");

            //force the validators to work
            if (ModelState.IsValid) {

                b.Filename = "";

                Book myBook = new Book()
                {
                    Name = b.Name.ToUpper(),
                    Year = b.Year,
                    Filename = b.Filename,
                    CategoryFK = b.CategoryFK,
                    Author = b.Author
                };

              _bookRepository.AddBook(myBook);
            }

            return View(b);
        
        }
    }
}
