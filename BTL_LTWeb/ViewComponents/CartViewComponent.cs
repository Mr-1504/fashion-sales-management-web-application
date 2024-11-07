using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BTL_LTWeb.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly QLBanDoThoiTrangContext _context;

        public CartViewComponent(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var count = 0;
            if (User.Identity.IsAuthenticated)
            {
                var email = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.Email)?.Value;
                count = await _context.TGioHangs.CountAsync(e => e.KhachHang.Email == email);
                int a = 0;
            }
            return View("RenderCart", count);
        }
    }
}
