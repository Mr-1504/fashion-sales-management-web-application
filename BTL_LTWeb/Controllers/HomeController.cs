using BTL_LTWeb.Models;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing.Printing;
using X.PagedList;

namespace BTL_LTWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLBanDoThoiTrangContext db;
        private readonly ILogger<HomeController> _logger;
        private int pageSize = 9;

        public HomeController(ILogger<HomeController> logger, QLBanDoThoiTrangContext doThoiTrangContext)
        {
            _logger = logger;
            db = doThoiTrangContext;
        }
        //action trang chủ
        public IActionResult Home()
        {
            var lst = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp).Take(5);
            return View(lst);
        }
        //action trang danh sách sản phẩm
        public IActionResult Index()
        {
            //int pageSize = 9;
            //int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            //var list = db.TDanhMucSps.AsNoTracking().OrderBy(x => x.TenSp);
            //PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
            //return View(lst);
            var Sp = db.TDanhMucSps.AsQueryable();
            int pageNum = (int)Math.Ceiling(Sp.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.keyword = "";
            var result = Sp.Take(pageSize).ToList();
            return View(result);
        }
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
            return PartialView("BangSanPhamHome", result);
        }
        public IActionResult Sanphamtheoloai(string Loai, int? Page)
        {
            int pageSize = 9;
            int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            var list = db.TDanhMucSps.AsNoTracking().Where(x => x.LoaiDt == Loai).OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
            ViewBag.Loai = Loai;
            return View(lst);
        }
        public IActionResult ChitietSp(int MaSP)
        {
            var sp = db.TDanhMucSps.SingleOrDefault(x => x.MaSp == MaSP);
            var anhSp = db.TAnhSps.Where(x => x.MaSp == MaSP).ToList();
            ViewBag.AnhSP = anhSp;
            return View(sp);
        }
        //action sử dụng viewModels
        public IActionResult ChitietSpNew(int MaSP)
        {
            var sp = db.TDanhMucSps.SingleOrDefault(x => x.MaSp == MaSP);
            var anhSp = db.TAnhSps.Where(x => x.MaSp == MaSP).ToList();
            var sizes = db.TChiTietSanPhams
                              .Where(x => x.MaSp == MaSP)
                              .Select(x => x.KichThuoc)
                              .Distinct() 
                              .ToList();
            var colors = db.TChiTietSanPhams
                              .Where(x => x.MaSp == MaSP)
                              .Select(x => x.MauSac)
                              .Distinct()
                              .ToList();
            HomeProductDetailViewModel model = new HomeProductDetailViewModel
            {
                product = sp,
                productImages = anhSp,
                Sizes = sizes,
                Colors = colors
            };

            return View(model);
        }
        public IActionResult TimSanPham(string Tensanpham, int? Page)
        {
            int pageSize = 9;
            int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
            var list = db.TDanhMucSps.AsNoTracking().Where(x => x.TenSp.Contains(Tensanpham)).OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
            ViewBag.Tensanpham = Tensanpham;
            return View(lst);
        }
        public IActionResult Sanphamtheogia(int Gia, int? Page)
        {

                var list = db.TDanhMucSps.AsNoTracking().Where(x => x.Gia <= Gia).OrderBy(x => x.Gia).ToList();
                int pageSize = 9;
                int pageNumber = Page == null || Page <= 0 ? 1 : Page.Value;
                PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(list, pageNumber, pageSize);
                ViewBag.Gia = Gia;
                return View(lst);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Map()
        {
            var stores = db.TDanhSachCuaHangs.AsNoTracking().ToList();
            return View(stores);
        }
        public IActionResult SearchStores(string city, string district)
        {
            var query = db.TDanhSachCuaHangs.AsQueryable();
            if (!string.IsNullOrEmpty(city))
            {
                query = query.AsEnumerable().Where(store => store.KhuVuc.Split(',')[1].Trim() == city).AsQueryable();
            }
            if (!string.IsNullOrEmpty(district))
            {
                query = query.AsEnumerable().Where(store => store.KhuVuc.Split(',')[0].Trim() == district).AsQueryable();
            }

            var stores = query.ToList();
            return PartialView("_StoreList", stores);
        }

        public JsonResult GetDistrictsByCity(string city)
        {
            var districts = db.TDanhSachCuaHangs
                              .AsEnumerable()
                              .Where(store => store.KhuVuc != null && store.KhuVuc.Split(',')[1].Trim() == city)
                              .AsEnumerable()
                              .Select(store => store.KhuVuc.Split(',')[0].Trim())
                              .Distinct()
                              .ToList();

            return Json(districts);
        }






    }
}
