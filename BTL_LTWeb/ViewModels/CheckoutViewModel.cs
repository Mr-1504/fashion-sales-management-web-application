using BTL_LTWeb.Models;

namespace BTL_LTWeb.ViewModels
{
    public class CheckoutViewModel
    {

        public IEnumerable<TGioHang> CartItems { get; set; }
        public TKhachHang CustomerInfo { get; set; }
        public string DiscountCode { get; set; }


    }
}
