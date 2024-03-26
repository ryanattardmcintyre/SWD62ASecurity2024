using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.ActionFilters
{
    public class BookAccessActionFilterAttribute : ActionFilterAttribute
    {
        private readonly bool _read;
      //  private readonly bool _write;

        public BookAccessActionFilterAttribute(bool read) =>
            (_read) = (read);

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string bookId = context.HttpContext.Request.Query["bookId"];
            if (string.IsNullOrEmpty(bookId) ||
                context.HttpContext.User.Identity.IsAuthenticated ==false
                )
            {
                context.Result = new ForbidResult();
            }

            PermissionsRepository pr = context.HttpContext.RequestServices.GetService<PermissionsRepository>();
            if (pr != null)
            {
                var list = pr.GetPermissions(Convert.ToInt32(bookId));

                if(list.Count(x=>x.User.Email == context.HttpContext.User.Identity.Name && x.Read == _read  
                ) > 0)
                {

                }
                else
                {
                    context.Result=new ForbidResult();
                }
            }
            else
            {
                context.Result = new NotFoundResult();
            }
            

            base.OnResultExecuting(context);
        }
    }
}
