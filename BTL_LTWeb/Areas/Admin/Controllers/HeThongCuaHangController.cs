
﻿using BTL_LTWeb.Areas.Admin.ViewModels;
using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;



namespace BTL_LTWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/HeThongCuaHang")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class HeThongCuaHangController : Controller
    {
        private readonly QLBanDoThoiTrangContext db;
        private readonly int pageSize = 3;

        public HeThongCuaHangController(QLBanDoThoiTrangContext doThoiTrangContext)
        {
            db = doThoiTrangContext;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            var shops = db.TDanhSachCuaHangs.AsQueryable();
            int pageNum = (int)Math.Ceiling(shops.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.keyword = "";
            var result = shops.Take(pageSize).ToList();
            return View(result);
        }

        [HttpGet]
        [Route("ShopsFilter")]
        public IActionResult ShopsFilter(string? keyword, int? pageIndex)
        {
            var shops = db.TDanhSachCuaHangs.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                shops = shops.Where(l => l.SDTCuaHang.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }
            int page = (pageIndex ?? 1);
            int pageNum = (int)Math.Ceiling(shops.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = shops.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return PartialView("ShopTable", result);
        }
        [Route("Delete")]
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var shop = db.TDanhSachCuaHangs.FirstOrDefault(s => s.SDTCuaHang == id);
            if (shop == null)
            {
                return NotFound();
            }
            db.TDanhSachCuaHangs.Remove(shop);
            db.SaveChanges();
            return Ok();
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TDanhSachCuaHang shop)
        {
            var existingShop = db.TDanhSachCuaHangs.FirstOrDefault(s => s.SDTCuaHang == shop.SDTCuaHang);
            if (existingShop != null)
            {

                ModelState.AddModelError("SDTCuaHang", "Số điện thoại cửa hàng đã có.");
                return View(shop);
            }
            if (ModelState.IsValid)
            {
                db.TDanhSachCuaHangs.Add(shop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shop);

        }
        [Route("Edit/{SDTCuaHang}")]
        [HttpGet]
        public IActionResult Edit(string SDTCuaHang)
        {
            var shop = db.TDanhSachCuaHangs.FirstOrDefault(s => s.SDTCuaHang == SDTCuaHang);
            if (shop == null)
            {
                return NotFound();
            }
            //var khuVucParts = shop.KhuVuc.Split(',');
            //ViewBag.Huyen = khuVucParts.Length > 0 ? khuVucParts[0].Trim() : "";
            //ViewBag.Tinh = khuVucParts.Length > 1 ? khuVucParts[1].Trim() : "";
            //ViewBag.Provinces = GetProvinces();
            //ViewBag.Districts = GetDistricts(khuVucParts[1].Trim());
            return View(shop);
        }
        [HttpPost]
        [Route("Edit/{SDTCuaHang}")]
        public IActionResult Edit(TDanhSachCuaHang shop)
        {
            if (ModelState.IsValid)
            {
                db.TDanhSachCuaHangs.Update(shop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shop);
        }
        //private List<Province> GetProvinces()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var response = client.GetStringAsync("https://provinces.open-api.vn/api/p/").Result;
        //        return JsonConvert.DeserializeObject<List<Province>>(response);
        //    }
        //}

        //private List<District> GetDistricts(string provinceCode)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var response = client.GetStringAsync($"https://provinces.open-api.vn/api/p/{provinceCode}?depth=2").Result;
        //        var data = JsonConvert.DeserializeObject<DistrictResponse>(response);
        //        return data.Districts;
        //    }
        //}
    }
}
