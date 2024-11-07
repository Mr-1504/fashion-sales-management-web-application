using BTL_LTWeb.Models;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BTL_LTWeb.Controllers
{
    public class GiamGiaController : Controller
    {
        private readonly QLBanDoThoiTrangContext _context;

        public GiamGiaController(QLBanDoThoiTrangContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
