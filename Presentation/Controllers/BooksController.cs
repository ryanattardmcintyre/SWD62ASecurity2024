using Common.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.Models;
using Presentation.Utilities;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace Presentation.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private BooksRepository _bookRepository;
        public BooksController(BooksRepository booksRepository) { 
        _bookRepository = booksRepository;
        }

        
        public async Task<IActionResult> Index([FromServices] UserManager<CustomUser> userRepository)
        {

            string passwordHash = (await userRepository.GetUserAsync(User)).Id.ToString();

            Encryption myEncryptionTool = new Encryption();

            var list = _bookRepository.GetAllBooks().ToList();
            


            //http://localhost:8080/books/delete?id=alskdjfl$aks|jdflask__


            //1. text data CONVERTED to utf32
            //2. encrypted the data
            //3. the encrypted data CONVERTED to base64


        
            var books = from b in list
                    select new BookViewModel()
                    {
                         EncryptedId = HttpUtility.UrlEncode(
                                       Convert.ToBase64String(myEncryptionTool.SymmetricEncrypt(
                                       System.Text.UTF32Encoding.UTF32.GetBytes(
                                       Convert.ToString(b.Id)), passwordHash))
                                       ),
                        Id = b.Id,
                        Author = b.Author,
                        CategoryFK = b.CategoryFK,
                      
                        Name = b.Name,
                        Year = b.Year
                    };


            //in the encryption:
            //step 1 UTF32 > byte[] //UTF32.GetBytes()
            //step 2 encryption/hashing/decryption
            //step 3 byte[] > base64 //Convert.ToBase64

            //in the decryption:
            //step 1 base64 > byte[] //Convert.FromBase64
            //step 2 ....
            //step 3 byte[] > UTF32 //UTF32.GetString()


          /*  foreach (var book in books)
            {

                byte[] encryptedData = myEncryptionTool.SymmetricEncrypt(UTF32Encoding.UTF32.GetBytes(Convert.ToString(book.Id)), passwordHash);
                book.EncryptedId = Convert.ToBase64String(encryptedData);
            }

            */
            return View(books);
        }



        [Authorize(Roles ="Librarian,Admin")]
        [HttpGet] //this is going to load a page with empty textboxes which the user can fill with data
        public IActionResult Create()
        {
            return View();
        }

        
        [Authorize(Roles = "Librarian,Admin")]
        [HttpPost] //this is going to be called when the user hits the submit button - this is saving the data into the db
        public IActionResult Create(BookViewModel b, [FromServices] IWebHostEnvironment host)
        {
            ModelState.Remove("Id");
         //   ModelState.Remove("Category");
            ModelState.Remove("Filename");
            ModelState.Remove("File");
            ModelState.Remove("EncryptedId");
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
        public async Task<ActionResult> Delete(string id, [FromServices]IHostEnvironment env, [FromServices] UserManager<CustomUser> userRepository)
        {
            //decrypt the received value

            string safeInput = HttpUtility.UrlDecode(id);

            int originalId = 0;
            //decrypt
            //1. text data CONVERTED to utf32 bytes
            //2. encrypted the data
            //3. the encrypted data CONVERTED to base64

            //---------------------------------------------------------------

            //3. base64 string CONVERTED into bytes

            try
            {
                byte[] cipherAsBytes = Convert.FromBase64String(safeInput);

                //2. decrypt the data

                string passwordHash = (await userRepository.GetUserAsync(User)).Id.ToString();
                byte[] originalDataAsBytes = new Encryption().SymmetricDecrypt(cipherAsBytes, passwordHash);

                //1. the decrypted data CONVERTED into UTF32
                originalId = Convert.ToInt32(UTF32Encoding.UTF32.GetString(originalDataAsBytes)); //THIS IS HIGHLY CRITICAL

                //delete 
                var bookToBeDeleted = _bookRepository.GetAllBooks().SingleOrDefault(x => x.Id == originalId);
                if (bookToBeDeleted != null)
                {
                    if (_bookRepository.GetBookPermissions(originalId).Count() == 0)
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
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to read the id";
                //log 
            }

            return RedirectToAction("Index");
        }


        [BookAccessActionFilter(true)]
        public IActionResult Download(int bookId, [FromServices] IHostEnvironment env)
        {
            string ipaddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

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
