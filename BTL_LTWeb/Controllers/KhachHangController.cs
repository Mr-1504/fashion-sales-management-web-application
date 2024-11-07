using BTL_LTWeb.Models;
using BTL_LTWeb.Services;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList;

namespace BTL_LTWeb.Controllers
{
    [Authorize(Roles = "KhachHang")]
    public class KhachHangController : Controller
    {
        QLBanDoThoiTrangContext db;
        private readonly EmailService _emailService;
        public KhachHangController(EmailService emailService, QLBanDoThoiTrangContext context)
        {
            _emailService = emailService;
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("KhachHang/Suathongtin")]
        [HttpGet]
        public IActionResult Suathongtin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var _kh = db.TKhachHangs.FirstOrDefault(kh => kh.Email == userEmail);
            if (_kh == null)
            {
                return NotFound();
            }

            return PartialView("Suathongtin", _kh);
        }

        [Route("KhachHang/Suathongtinajax")]
        [HttpGet]
        public IActionResult SuathongtinAjax()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var _kh = db.TKhachHangs.FirstOrDefault(kh => kh.Email == userEmail);
            if (_kh == null)
            {
                return NotFound();
            }

            return PartialView("_suaPartial", _kh);
        }

        [Route("KhachHang/Suathongtin")]
        [HttpPost]
        public IActionResult Suathongtin(TKhachHang kh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userEmail = User.FindFirstValue(ClaimTypes.Email);
                    var _kh = db.TKhachHangs.FirstOrDefault(kh => kh.Email == userEmail);

                    if (_kh != null)
                    {
                         if(_kh.Email != kh.Email)
                        {
                            return Json(new { success = false, message = "Không thể thay đổi địa chỉ email!" });
                        }   
                        _kh.TenKhachHang = kh.TenKhachHang;
                        _kh.NgaySinh = kh.NgaySinh;
                        _kh.SoDienThoai = kh.SoDienThoai;
                        _kh.DiaChi = kh.DiaChi;

                        db.SaveChanges(); 
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Khách hàng không tồn tại." });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error saving changes: " + ex.Message);
                    return Json(new { success = false, message = "Có lỗi xảy ra khi lưu dữ liệu." });
                }
            }

            return Json(new { success = false, message = "Cập nhật thông tin thất bại!" });
        }

        [Route("KhachHang/DonHang")]
        public IActionResult DonHang(int? Page, string? searchTerm)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var _kh = db.TKhachHangs.FirstOrDefault(kh => kh.Email == userEmail);

            if (_kh == null)
            {
                return NotFound();
            }
            int _khID = _kh.MaKhachHang;
            var lst = db.THoaDonBans
                        .Where(dh => dh.MaKhachHang == _khID);

            if (!string.IsNullOrEmpty(searchTerm))
            {  
                DateTime searchDate;
                bool isDate = DateTime.TryParse(searchTerm, out searchDate);

                lst = lst.Where(dh =>
                    dh.MaHoaDonBan.ToString().Contains(searchTerm) ||
                    (isDate && dh.NgayHoaDon.HasValue && dh.NgayHoaDon.Value.Date == searchDate.Date) ||
                    dh.PhuongThucThanhToan.Contains(searchTerm)
                );
            }
            lst = lst.OrderByDescending(dh => dh.NgayHoaDon).AsNoTracking();

            int pageSize = 5;
            int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            int totalItems = lst.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedData = lst
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;
            if (!pagedData.Any())
            {
                ViewBag.Notification = "Không tìm thấy đơn hàng nào!";
            }
            else
            {
                ViewBag.Notification = null; 
            }

            return PartialView("DonHang",pagedData);
        }

        public IActionResult ChiTietDonHang(int MaDH)
        {
            var hoaDon = db.THoaDonBans.FirstOrDefault(dh => dh.MaHoaDonBan == MaDH);
            if (hoaDon == null)
            {
                return NotFound();
            }
            var chiTietHoaDon = db.TChiTietHoaDonBans.Where(ct => ct.MaHoaDonBan == MaDH).Select(ct => new
            {
                ct.SoLuongBan,
                ct.DonGiaBan,
                ct.MaChiTietSP
            }).ToList();

            var dsSP = db.TChiTietSanPhams.Where(sp => chiTietHoaDon.Select(ct => ct.MaChiTietSP).Contains(sp.MaChiTietSp)).Select(sp => new
            {
                sp.MaChiTietSp,
                sp.MaSp,
            }).ToList();

            var tenSP = db.TDanhMucSps.Where(m => dsSP.Select(sp => sp.MaSp).Contains(m.MaSp)).Select(m => new
            {
                m.MaSp,
                m.TenSp
            }).ToList();

            var danhSach = (from ct in chiTietHoaDon
                            join sp in dsSP on ct.MaChiTietSP equals sp.MaChiTietSp
                            join ten in tenSP on sp.MaSp equals ten.MaSp
                            select new ProductDetailViewModel
                            {
                                TenSP = ten.TenSp,
                                SoLuong = (int)ct.SoLuongBan,
                                DonGia = (decimal)ct.DonGiaBan
                            }).ToList();

            var viewModel = new ChiTietDonHangViewModel
            {
                MaHoaDonBan = hoaDon.MaHoaDonBan,
                NgayHoaDon = (DateTime)hoaDon.NgayHoaDon,
                PhuongThucThanhToan = hoaDon.PhuongThucThanhToan,
                TongTienHd = (decimal)hoaDon.TongTienHd,
                ChiTietSanPhams = danhSach
            };
            return View(viewModel);
        }

        public async Task<IActionResult> UpdatePassword()
        {
            return PartialView("UpdatePassword");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel update)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await db.TUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("Lỗi hệ thống, thử lại sau");
            }
            if(SecurityService.HashPasswordWithSalt(update.CurrentPassword, user.Salt) != user.Password)
            {
                return BadRequest("Mật khẩu hiện tại không đúng.");
            }
            if (update.NewPassword != update.ConfirmPassword)
            {
                return BadRequest("Mật khẩu xác nhận không khớp.");
            }
            user.Password = SecurityService.HashPasswordWithSalt(update.NewPassword, user.Salt);
            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
