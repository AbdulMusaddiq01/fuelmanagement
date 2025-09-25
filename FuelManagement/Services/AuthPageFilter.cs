using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FuelManagement.Services
{
    public class AuthPageFilter : IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var path = httpContext.Request.Path.Value?.ToLower();
            var userdetails = httpContext.Session.GetString("userdetails");

            
            if (path.Contains("/login") && string.IsNullOrEmpty(userdetails))
            {
                return;
            }

            if (string.IsNullOrEmpty(userdetails))
            {
                context.Result = new RedirectToPageResult("/login");
            }
            //else
            //{
            //    context.Result = new RedirectToPageResult("/Index");
            //}

           
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            
        }
    }
}
