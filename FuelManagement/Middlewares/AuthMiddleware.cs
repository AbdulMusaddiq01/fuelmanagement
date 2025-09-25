
namespace FuelManagement.Middlewares
{
    public class AuthMiddleware : IMiddleware
    {
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Allow static files and login page
            if (path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/images") ||
                path.Contains("/login"))
            {
                await next(context);
                return;
            }

            if (!path.Contains("/login") && string.IsNullOrEmpty(context.Session.GetString("userdetails")))
            {
                context.Response.Redirect("/login");
                return;
            }else
            {
                context.Response.Redirect("/Index");
                return;
            }

           await next(context);
        }
    }
}
