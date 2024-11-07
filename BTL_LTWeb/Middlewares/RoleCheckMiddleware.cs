namespace BTL_LTWeb.Middlewares
{
    public class RoleCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                if (context.Session.GetString("HasRedirected") != "true")
                {
                    context.Session.SetString("HasRedirected", "true");

                    if (context.User.IsInRole("KhachHang"))
                    {
                        if (!context.Request.Path.StartsWithSegments("/Home/Home"))
                        {
                            context.Response.Redirect("/Home/Home");
                            return;
                        }
                    }
                    else
                    {
                        if (!context.Request.Path.StartsWithSegments("/Admin/Index"))
                        {
                            context.Response.Redirect("/Admin/Index");
                            return;
                        }
                    }
                }
            }
            await _next(context);
        }
    }
}
