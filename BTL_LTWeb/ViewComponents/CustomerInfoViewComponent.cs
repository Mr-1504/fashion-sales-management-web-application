using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BTL_LTWeb.ViewModels;

namespace BTL_LTWeb.ViewComponents
{
    public class CustomerInfoViewComponent : ViewComponent
    {
        QLBanDoThoiTrangContext _context;
        public CustomerInfoViewComponent(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var _user = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(_user))
            {
                return Content("Không có khách hàng nào đang đăng nhập");
            }
            var _customer = _context.TKhachHangs.FirstOrDefault(k => k.Email == _user);
            if (_customer == null)
            {
                return Content("Khách hàng không tồn tại!");
            }

            var totalAmount = _context.THoaDonBans.Where(h => h.MaKhachHang == _customer.MaKhachHang).Sum(h => (decimal?)h.TongTienHd);
            var points = (totalAmount ?? 0) / 10000;
            var model = new CustomerInfoViewModel
            {
                _fullName = _customer.TenKhachHang,
                _userName = _customer.Email,
                _points = points
            };

            return View("RenderCustomerInfo", model);
        }
    }
}
