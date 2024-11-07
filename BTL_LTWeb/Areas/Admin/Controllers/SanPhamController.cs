using BTL_LTWeb.Models;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BTL_LTWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/SanPham")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class SanPhamController : Controller
    {
        private readonly QLBanDoThoiTrangContext db;
        private readonly int pageSize = 7;

        public SanPhamController(QLBanDoThoiTrangContext qLBanDoThoiTrangContext)
        {
            db = qLBanDoThoiTrangContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("danhmucsanpham")]
        public IActionResult danhmucsanpham()
        {
            var Sp = db.TDanhMucSps.AsQueryable();
            int pageNum = (int)Math.Ceiling(Sp.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.keyword = "";
            var result = Sp.Take(pageSize).ToList();
            return View(result);
        }
        [HttpGet]
        [Route("LocSanPham")]
        public IActionResult LocSanPham(string? keyword, int? pageIndex)
        {
            var sp = db.TDanhMucSps.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                sp = sp.Where(l => l.TenSp.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }
            int page = (pageIndex ?? 1);
            int pageNum = (int)Math.Ceiling(sp.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = sp.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return PartialView("BangSanPham", result);
        }

        [Route("Themsanpham")]
        [HttpGet]
        public IActionResult Themsanpham()
        {
            ViewBag.TagId = new SelectList(db.Tags, "TagId", "TagName");
            return View();
        }
        [HttpPost]
        [Route("Themsanpham")]
        public async Task<IActionResult> ThemsanphamAsync(TDanhMucSp sp, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {

                    string targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string targetFilePath = Path.Combine(targetDirectory, file.FileName);


                    using (var stream = new FileStream(targetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }


                    sp.AnhDaiDien = file.FileName;
                }

                db.TDanhMucSps.Add(sp);
                db.SaveChanges();
                return RedirectToAction("danhmucsanpham");
            }
            return View(sp);
        }
        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int MaSP)
        {
            ViewBag.TagId1 = new SelectList(db.Tags.ToList(), "TagId", "TagName");
            var sp = db.TDanhMucSps.Find(MaSP);
            return View(sp);
        }
        [HttpPost]
        [Route("SuaSanPham")]
        public async Task<IActionResult> SuaSanPham(TDanhMucSp sp, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                if (file != null && file.Length > 0)
                {
                    string targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string targetFilePath = Path.Combine(targetDirectory, file.FileName);


                    using (var stream = new FileStream(targetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    sp.AnhDaiDien = file.FileName;
                }

                db.Entry(sp).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("danhmucsanpham");
            }
            return View(sp);
        }
        // ...

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(int MaSP)
        {
            TempData["Log"] = "";
            var chitietsp = db.TChiTietSanPhams.Where(x => x.MaSp == MaSP).ToList();
            if (chitietsp.Count > 0)
            {
                TempData["Log"] = "Xóa thất bại";
                return RedirectToAction("danhmucsanpham");
            }
            var anhSp = db.TAnhSps.Where(x => x.MaSp == MaSP).ToList();
            if (anhSp.Any())
            {
                db.RemoveRange(anhSp);
            }
            var sp = db.TDanhMucSps.Find(MaSP);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sp.AnhDaiDien);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            db.Remove(sp);
            db.SaveChanges();
            TempData["Log"] = "Xóa thành công";
            return RedirectToAction("danhmucsanpham");
        }

        [Route("Timsanpham")]
        public IActionResult TimSanPham(string Tensanpham, int? Page)
        {
            int pageSize = 9;
            int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            var list = db.TDanhMucSps.AsNoTracking().Where(x => x.TenSp.Contains(Tensanpham)).OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
            return View(lst);
        }
        [Route("Timsanphamnew")]
        public IActionResult TimSanPhamNew(string Tensanpham, int? Page)
        {
            int pageSize = 9;
            int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            var list = db.TDanhMucSps.AsNoTracking().Where(x => x.TenSp.Contains(Tensanpham)).OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
            return PartialView("BangSanPham", lst);
        }
        [Route("Timsanphamnew1")]
        public IActionResult TimSanPhamNew1(string Tensanpham, int? Page)
        {
            int pageSize = 9;
            int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            var list = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
            return PartialView("BangSanPham", lst);
        }
        //chi tiết sản phẩm
        [Route("ChiTietSanPham")]
        [HttpGet]
        public IActionResult ChiTiet(int MaSP)
        {

            var sp = db.TDanhMucSps.Find(MaSP);
            var chitiet = db.TChiTietSanPhams.Where(x => x.MaSp == MaSP).ToList();
            ChiTietSanPhamViewModel chiTietsp = new ChiTietSanPhamViewModel
            {
                Sp = sp,
                chiTietSanPhams = chitiet
            };
            return View(chiTietsp);
        }
        [HttpPost]
        [Route("ChiTietSanPham")]
        public IActionResult ChiTiet(ChiTietSanPhamViewModel chiTietsp)
        {
            return View(chiTietsp);
        }
        // thêm chi tiết sản phẩm
        [Route("ThemChiTiet")]
        [HttpGet]
        public IActionResult ThemChiTiet(int MaSP)
        {
            var sp = db.TDanhMucSps.Find(MaSP);
            ViewBag.TenSP = sp.TenSp;
            ViewBag.MaSP = MaSP;
            return View();
        }
        [HttpPost]
        [Route("ThemChiTiet")]
        public IActionResult ThemChiTiet(TChiTietSanPham chitiet)
        {
            if (ModelState.IsValid)
            {
                db.TChiTietSanPhams.Add(chitiet);
                db.SaveChanges();
                return RedirectToAction("danhmucsanpham");
            }
            return View(chitiet);
        }
        //xoá chi tiết sản phẩm
        [HttpGet]
        [Route("XoaChiTiet")]
        public IActionResult XoaChiTiet(int MaChiTietSp)
        {
            var chitiet = db.TChiTietSanPhams.Find(MaChiTietSp);
            db.TChiTietSanPhams.Remove(chitiet);
            db.SaveChanges();
            TempData["LogChiTiet"] = "Xoá thành công";
            return RedirectToAction("ChiTiet", new { MaSP = chitiet.MaSp });
        }
    }
}
