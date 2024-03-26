using Common.Models;
using DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PermissionsRepository
    {
        LibraryContext _context;
        public PermissionsRepository(LibraryContext context) { 
            _context=context;
        }
        public IQueryable<Permission> GetPermissions(int bookId)
        {
            return _context.Permissions.Where(x => x.BookIdFK == bookId);
        
        }
    }
}
