using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BTL_LTWeb.ViewComponents
{
    public class TagViewComponent : ViewComponent
    {
        QLBanDoThoiTrangContext _context;
        public TagViewComponent(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tags = _context.Tags.AsQueryable();
            return View("RenderTag",tags);
        }
    }
}
