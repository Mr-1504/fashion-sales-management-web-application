using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BTL_LTWeb.ViewComponents
{
    public class LoaiLatestViewComponent : ViewComponent
    {
        QLBanDoThoiTrangContext _context;
        public LoaiLatestViewComponent(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loaiList = _context.TDanhMucSps.Select(d => d.LoaiDt).Distinct().ToList();
            return View("RenderLoaiLatest", loaiList);
        }
    }
}
