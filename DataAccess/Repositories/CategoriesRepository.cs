using Common.Models;
using DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CategoriesRepository
    {
        private LibraryContext _context;
        public CategoriesRepository(LibraryContext context)
        {

            _context = context;
        }

        public IQueryable<Category> GetCategories() {
            return _context.Categories;
        }
    }
}
