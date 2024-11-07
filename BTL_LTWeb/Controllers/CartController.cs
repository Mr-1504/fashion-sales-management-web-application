using BTL_LTWeb.Models;
using BTL_LTWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BTL_LTWeb.Controllers
{
    [Route("gio-hang")]
    public class CartController : Controller
    {
        private readonly QLBanDoThoiTrangContext _context;
        public CartController(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "KhachHang")]
        public async Task<IActionResult> Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var KhacHang = await _context.TKhachHangs.FirstOrDefaultAsync(e => e.Email == email);

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = await _context.TGioHangs
            .Include(x => x.ChiTietSanPham)
            .ThenInclude(x => x.DanhMucSp)
            .Where(x => x.MaKhachHang == KhacHang.MaKhachHang)
            .ToListAsync();

            return View(gioHang);
        }

        [Route("items")]
        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var khachHang = await _context.TKhachHangs.FirstOrDefaultAsync(e => e.Email == email);
            var cartItems = await _context.TGioHangs
                                           .Where(x => x.MaKhachHang == khachHang.MaKhachHang)
                                           .Include(x => x.ChiTietSanPham)
                                           .ThenInclude(x => x.DanhMucSp)
                                           .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound();
            }

            return PartialView("_ListCart", cartItems);
        }

        [Route("them-vao-gio-hang")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int Masp, string Size, string Color, int Soluong)
        {
            if (!User.Identity.IsAuthenticated)
                return BadRequest("Vui lòng đăng nhập trước khi thực hiện");
            if (!User.IsInRole("KhachHang"))
            {
                return BadRequest("Chỉ khách hàng mới có thể thêm vào giỏ hàng");
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var KhacHang = await _context.TKhachHangs.FirstOrDefaultAsync(e => e.Email == email);
            if (KhacHang == null)
                return BadRequest("Lỗi hệ thống! Thử lại sau");
            if (Size == null || Color == null)
                return BadRequest("Vui lòng chọn đủ thông tin");
            var detail = await _context.TChiTietSanPhams.FirstOrDefaultAsync(
                x => x.MaSp == Masp &&
                x.KichThuoc == Size &&
                x.MauSac == Color
            );
            if (detail == null)
                return NotFound("Sản phẩm không có sẵn hoặc đã hết hàng");
            var product = await _context.TGioHangs.FirstOrDefaultAsync(
                x => x.MaChiTietSP == detail.MaChiTietSp &&
                x.MaKhachHang == KhacHang.MaKhachHang
            );
            if (product == null)
            {
                TGioHang gioHang = new TGioHang
                {
                    MaKhachHang = KhacHang.MaKhachHang,
                    MaChiTietSP = detail.MaChiTietSp,
                    SoLuong = Soluong
                };
                _context.TGioHangs.Add(gioHang);
            }
            else
            {
                product.SoLuong += Soluong;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Route("so-luong")]
        [HttpGet]
        public async Task<IActionResult> GetCartItemAmount()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var khachHang = await _context.TKhachHangs.FirstOrDefaultAsync(e => e.Email == email);
            var cartItems = await _context.TGioHangs
                                           .Where(x => x.MaKhachHang == khachHang.MaKhachHang)
                                           .Include(x => x.ChiTietSanPham)
                                           .ThenInclude(x => x.DanhMucSp)
                                           .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound();
            }
            return Json(cartItems.Count);
        }

        [Route("Qr-code")]
        public ActionResult ShowImage()
        {
            ViewBag.ImageData = VietQrGenerator.GetQR(200001, "Truong van minh chuyen tien");

            return View();
        }

    }
}
