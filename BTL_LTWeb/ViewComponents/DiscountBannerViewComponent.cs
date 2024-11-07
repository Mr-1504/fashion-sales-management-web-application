using BTL_LTWeb.Models;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTL_LTWeb.ViewComponents
{
    public class DiscountBannerViewComponent : ViewComponent
    {
        private readonly QLBanDoThoiTrangContext _context;
        public DiscountBannerViewComponent(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var discounts = await _context.TMaGiamGias.Where(g => g.TrangThai == 1 && g.NgayKetThuc > DateTime.Now).OrderBy(g => g.NgayBatDau).FirstOrDefaultAsync();
            if (discounts == null)
            {
                return Content("Không có mã giảm giá nào đang hoạt động.");
            }

            var model = new DiscountBannerViewModel
            {
                Code = discounts.Code,
                Description = discounts.Mota,
                DiscountPercentage = (int)(discounts.TiLeGiam * 100),
                StartDate = (DateTime)discounts.NgayBatDau,
                EndDate = (DateTime)discounts.NgayKetThuc
            };
            return View("RenderDiscountBanner", model);
        }
    }
}
