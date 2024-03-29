﻿using Common.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.Models;

namespace Presentation.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private BooksRepository _bookRepository;
        public BooksController(BooksRepository booksRepository) { 
        _bookRepository = booksRepository;
        }

        
        public IActionResult Index()
        {
            var list = _bookRepository.GetAllBooks().ToList();
            
            var books = from b in list
                    select new BookViewModel()
                    {
                        Id = b.Id,
                        Author = b.Author,
                        CategoryFK = b.CategoryFK,
                      
                        Name = b.Name,
                        Year = b.Year
                    };

            return View(books);
        }



        [Authorize(Roles ="Librarian,Admin")]
        [HttpGet] //this is going to load a page with empty textboxes which the user can fill with data
        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian,Admin")]
        [HttpPost] //this is going to be called when the user hits the submit button - this is saving the data into the db
        public IActionResult Create(BookViewModel b, [FromServices] IWebHostEnvironment host)
        {
            ModelState.Remove("Id");
         //   ModelState.Remove("Category");
            ModelState.Remove("Filename");
            b.Filename = "";

            string message = "";

            //force the validators to work
            if (ModelState.IsValid) {

                //  Filtering against a whitelist the file --------------------------

                if (b.File != null)
                {
                    if (b.File.Length > 0 && b.File.Length < (10 * 1024 * 1024))
                    {

                        //37 80 68 70	
                        int[] whitelistPdf = new int[] { 37, 80, 68, 70 }; //magic numbers
                        bool fileCheck = true;
                        int counter = 0;
                        int myReadByte;

                        using (var stream = b.File.OpenReadStream())
                        {
                                do
                                {
                                    myReadByte = stream.ReadByte();
                                    if (myReadByte == -1)
                                    {
                                    fileCheck = false;
                                    }
                                    else if (whitelistPdf[counter] != myReadByte)
                                    {
                                        fileCheck = false;
                                    }
                                    counter++;
                                 } while (fileCheck == true && counter < 4);

                                stream.Position = 0;
                        }

                        if (fileCheck == false)
                        {
                            TempData["error"] = "File not allowed; only pdfs are";
                            ModelState.AddModelError("File", "File not allowed; only pdfs are");
                        }
                        else
                        {

                            string filename = Guid.NewGuid().ToString() + ".pdf";
                            //to save a file physically we need the absolute path

                            //C:\Users\attar\source\repos\swd62asecurity2024\Solution1\Presentation
                            string absolute = host.ContentRootPath + "data\\" + filename;


                            MemoryStream myFile = new MemoryStream();
                            b.File.CopyTo(myFile);
                            myFile.Position = 0;
                            System.IO.File.WriteAllBytes(absolute, myFile.ToArray());

                            //filename has to be assigned into b
                            b.Filename = "\\data\\" + filename;
                            message += "File uploaded successfully; ";
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("File", "File Size is not allowed. File must be lower than 10MB");
                        TempData["error"] = "File Size is not allowed. File must be lower than 10MB";
                    }
                }
                 
                //  Saving in database -----------------------------------------
       

                Book myBook = new Book()
                {
                    Name = b.Name.ToUpper(),
                    Year = b.Year,
                    Filename = b.Filename,
                    CategoryFK = b.CategoryFK,
                    Author = b.Author
                };

              _bookRepository.AddBook(myBook);
                message += "Details saved in database successfully";
                TempData["message"] = message;
            }

            return View(b);
        
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, [FromServices]IHostEnvironment env)
        {
            //delete 
            var bookToBeDeleted = _bookRepository.GetAllBooks().SingleOrDefault(x => x.Id == id);
            if (bookToBeDeleted != null)
            {
                if (_bookRepository.GetBookPermissions(id).Count() == 0)
                {//
                    _bookRepository.DeleteBook(bookToBeDeleted);

                    //Delete the physical file as well
                    string absolutePath = env.ContentRootPath + "\\" + bookToBeDeleted.Filename;
                    if (System.IO.File.Exists(absolutePath))
                    {
                      System.IO.File.Delete(absolutePath);
                    }
                    TempData["message"] = "Book deleted";
                }
                else
                {
                    TempData["error"] = "Book cannot be deleted because other users have access to it";
                }
            }
            else
            {
                TempData["error"] = "Book not in the database";
            }

            return RedirectToAction("Index");
        }


        [BookAccessActionFilter(true)]
        public IActionResult Download(int bookId, [FromServices] IHostEnvironment env)
        {
            var book = _bookRepository.GetAllBooks().SingleOrDefault(x => x.Id == bookId);
            if(book != null)
            {
                //Data/filename.pdf
                string absolutePathToTheBook = env.ContentRootPath + book.Filename;

               byte[] bookInBytes =System.IO.File.ReadAllBytes(absolutePathToTheBook);
                return File(bookInBytes, "application/pdf", Guid.NewGuid() + ".pdf");

               
            }
            else
            {
                TempData["error"] = "Book not found";
                return RedirectToAction("Index");
            }


        }



    }
}
