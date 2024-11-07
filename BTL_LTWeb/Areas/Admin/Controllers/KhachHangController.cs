using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using X.PagedList;
using static System.Formats.Asn1.AsnWriter;

namespace BTL_LTWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/KhachHang")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class KhachHangController : Controller
    {
        QLBanDoThoiTrangContext db;
        public KhachHangController(QLBanDoThoiTrangContext context)
        {
            db = context;
        }
        private int pageSize = 6;
        public IActionResult Index()
        {
            return View();
        }
        // khách hàng
        [HttpGet]
        [Route("danhsachkhachhang")]
        public IActionResult danhsachkhachhang()
        {
            var kh = db.TKhachHangs.AsNoTracking().ToList();
            int pageNum = (int)Math.Ceiling(kh.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.keyword = "";
            var result = kh.Take(pageSize).ToList();
            return View(result);
        }

        [Route("KhachHangFilter")]
        public IActionResult KhachHangFilter(string? keyword, int? pageIndex)
        {
            var kh = db.TKhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                kh = kh.Where(l => l.TenKhachHang.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }
            int page = (pageIndex ?? 1);
            int pageNum = (int)Math.Ceiling(kh.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = kh.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            if(result == null || !result.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy khách hàng!" });
            }
            return PartialView("KhachHangTable", result);
        }

        [Route("Danhsachhoadonkhachhang")]
        [HttpGet]
        public IActionResult ChiTietKhachHang(int maKH, int? page)
        {
             pageSize = 2;
            int pageNumber = page ?? 1;
            var _kh = db.TKhachHangs.AsNoTracking().FirstOrDefault(x => x.MaKhachHang == maKH);
            if (_kh == null)
            {
                return NotFound("Khách hàng không tồn tại!");
            }
            var lst = db.THoaDonBans
                         .AsNoTracking()
                         .Where(x => x.MaKhachHang == maKH)
                         .OrderByDescending(x => x.MaHoaDonBan);

            if (!lst.Any())
            {
                ViewBag.ThongBao = "Không có dữ liệu hóa đơn cho khách hàng này!";
            }
            ViewBag.KhachHang = _kh;

            bool isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                ViewBag.PageNum = (int)Math.Ceiling(lst.Count() / (double)pageSize);
                var pagedList = lst.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return PartialView("HoaDonKhachHangPartial", pagedList); 
            }

            ViewBag.PageNum = (int)Math.Ceiling(lst.Count() / (double)pageSize);
            var fullList = lst.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return View(fullList);
        }

        
    }
}
