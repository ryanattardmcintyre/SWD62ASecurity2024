using Common.Models;
using DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class BooksRepository
    {
        //Dependency Injection is used to keep the instances needed among the controllers, classes, and methods (i.e services)
        //to the minimum

        private LibraryContext _context;
        public BooksRepository(LibraryContext context) {

            _context = context;
        }

        public void AddBook(Book b)
        {
            _context.Books.Add(b);
            _context.SaveChanges();
          
        }

        public IQueryable<Book> GetAllBooks()
        {
            return _context.Books;

        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }


        public IQueryable<Permission> GetBookPermissions(int id)
        {
            return _context.Permissions.Where(x => x.BookIdFK == id);
        }
    }
}
