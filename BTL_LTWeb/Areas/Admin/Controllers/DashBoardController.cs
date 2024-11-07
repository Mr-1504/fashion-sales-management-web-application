using BTL_LTWeb.Models;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BTL_LTWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/DashBoard")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class DashBoardController : Controller
    {
        QLBanDoThoiTrangContext db;
        public DashBoardController(QLBanDoThoiTrangContext context)
        {
            db = context;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult DashBoard()
        {
            
            var topProducts = db.TChiTietHoaDonBans
                            .GroupBy(x => x.MaChiTietSP)
                            .Select(g => new
            {
                MaChiTietSP = g.Key,
                TotalRevenue = g.Sum(x => x.DonGiaBan * x.SoLuongBan)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .Take(4)
            .ToList();
            var Sanpham = (from hdb in topProducts
                           join ctp in db.TChiTietSanPhams on hdb.MaChiTietSP equals ctp.MaChiTietSp
                           join dm in db.TDanhMucSps on ctp.MaSp equals dm.MaSp
                           select dm).Distinct().ToList();
            var doanhThuNam = db.THoaDonBans.Where(x => x.NgayHoaDon.Value.Year == DateTime.Now.Year).Sum(x => x.TongTienHd);
            var doanhthuthang = db.THoaDonBans.Where(x => x.NgayHoaDon.Value.Month == DateTime.Now.Month).Sum(x => x.TongTienHd);
            var doanhthuthangtruoc = db.THoaDonBans.Where(x => x.NgayHoaDon.Value.Month == DateTime.Now.Month-1).Sum(x => x.TongTienHd);
            var doanhthuthangtruoc1 = db.THoaDonBans.Where(x => x.NgayHoaDon.Value.Month == DateTime.Now.Month-2).Sum(x => x.TongTienHd);
            HoaDonBanViewModel model = new HoaDonBanViewModel
            {
                danhMucSp = Sanpham,
                doanhThuNam = doanhThuNam,
                doanhThuThangNay = doanhthuthang,
                doanhThuThangTruoc = doanhthuthangtruoc,
                doanhThuThangTruoc1 = doanhthuthangtruoc1
            };
            return View(model);
        }
    }
}
