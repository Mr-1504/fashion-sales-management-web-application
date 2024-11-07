using BTL_LTWeb.Models;
using BTL_LTWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using X.PagedList;

namespace BTL_LTWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NhanVien")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class NhanVienController : Controller
    {
        QLBanDoThoiTrangContext db;
        public NhanVienController(QLBanDoThoiTrangContext context)
        {
            db = context;
        }
        private int pageSize = 6;
        public IActionResult Index()
        {
            return View();
        }
        //nhân viên
        [Route("danhsachnhanvien")]
        public IActionResult danhsachnhanvien(int? Page)
        {
            var nv = db.TNhanViens.AsNoTracking().ToList();
            int pageNum = (int)Math.Ceiling(nv.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.keyword = "";
            var result = nv.Take(pageSize).ToList();
            return View(result);
        }

        [Route("NhanVienFilter")]
        public IActionResult NhanVienFilter(string? keyword, int? pageIndex)
        {
            var nv = db.TNhanViens.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                nv = nv.Where(l => l.TenNhanVien.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }
            int page = (pageIndex ?? 1);
            int pageNum = (int)Math.Ceiling(nv.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = nv.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            if (result == null || !result.Any())
            {
                return Json(new { success = false, message = "Không tìm thấy nhân viên!" });
            }
            return PartialView("NhanVienTable", result);
        }

        [Route("ThemNhanVien")]
        [HttpGet]
        public IActionResult Themnhanvien()
        {
            return View("Themnhanvien");
        }
        [HttpPost]
        [Route("ThemNhanVien")]
        public IActionResult Themnhanvien(TNhanVien nv)
        {
            if (ModelState.IsValid)
            {
                var user = db.TUsers.FirstOrDefault(u => u.Email == nv.Email);
                if(user != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại. Không thể tạo tài khoản bằng email này!");
                    return View(nv);
                }
               
               
                string salt = SecurityService.GenerateSalt();
                string hash = SecurityService.HashPasswordWithSalt("Abc1234@", salt);
                user = new TUser
                {
                    Email = nv.Email,
                    Password = hash,
                    Salt = salt,
                    LoaiUser = "NhanVien"
                };
                db.TUsers.Add(user);
                db.SaveChanges();
                
                db.TNhanViens.Add(nv);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                return RedirectToAction("danhsachnhanvien", "NhanVien");
            }
            return View(nv);
        }

        [Route("Suanhanvien")]
        [HttpGet]
        public IActionResult Suanhanvien(int MaNV)
        {
            var nv = db.TNhanViens.Find(MaNV);
            return View(nv);
        }
        [HttpPost]
        [Route("Suanhanvien")]
        public IActionResult Suanhanvien(TNhanVien nv)
        {
            if (ModelState.IsValid)
            {
                var existingnv = db.TNhanViens.Find(nv.MaNhanVien);
                if (existingnv == null)
                {
                    ModelState.AddModelError("MaNhanVien", "Nhân viên không tồn tại");
                    return View(nv);
                }
                
                if (existingnv.Email != nv.Email)
                {
                    ModelState.AddModelError("Email", "Bạn không được phép thay đổi email.");
                    nv.Email = existingnv.Email;
                    return View(nv);
                }
                existingnv.Email = nv.Email;
                existingnv.TenNhanVien = nv.TenNhanVien;
                existingnv.NgaySinh = nv.NgaySinh;
                existingnv.ChucVu = nv.ChucVu;
                existingnv.GhiChu = nv.GhiChu;
                db.Entry(existingnv).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction("danhsachnhanvien","NhanVien");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
            return View(nv);
        }

        [Route("XoaNhanVien")]
        [HttpPost]
        public IActionResult XoaNhanVien(int MaNV)
        {

            var nv = db.TNhanViens.FirstOrDefault(x => x.MaNhanVien == MaNV);
            if (nv != null)
            {
                try
                {
                    var email = nv.Email; 
                    Console.WriteLine("Email nhân viên: " + email);
                    var user = db.TUsers.FirstOrDefault(g => g.Email == email);
                    db.TNhanViens.Remove(nv);
                    if (user != null)
                    {
                        Console.WriteLine(user.Email);
                        db.TUsers.Remove(user);
                    }
                    else
                    {
                        Console.WriteLine("User không tìm thấy với email: " + nv.Email);  
                    }
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa nhân viên thành công." });
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", "Lỗi khi xóa nhân viên và user: " + ex.Message);
                    //return RedirectToAction("danhsachnhanvien");
                    Console.WriteLine("Lỗi: " + ex.ToString());
                    return Json(new { success = false, message = "Lỗi khi xóa nhân viên và user: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "Nhân viên không tồn tại." });
            }
            
        }

        //[Route("TimNhanVien")]
        //[HttpGet]
        //public IActionResult TimNhanVien(string TenNhanVien, int? Page)
        //{
            
        //    int pageSize = 6;
        //    int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;

        //    var list = db.TNhanViens.AsNoTracking().Where(x => string.IsNullOrEmpty(TenNhanVien) || x.TenNhanVien.Contains(TenNhanVien)).OrderBy(x => x.MaNhanVien);

        //    var pagedList = new PagedList<TNhanVien>(list, pageNumber, pageSize);

        //    if (!pagedList.Any())
        //    {
        //        return Content(string.Empty);
        //    }
        //    ViewBag.TenNhanVien = TenNhanVien;

        //    return PartialView("_DanhSachNhanVienPartial", pagedList);
        //}
    }
}
