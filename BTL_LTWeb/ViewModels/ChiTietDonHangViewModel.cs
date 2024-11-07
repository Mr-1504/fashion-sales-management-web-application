using BTL_LTWeb.Models;

namespace BTL_LTWeb.ViewModels
{
    public class ChiTietDonHangViewModel
    {
        public int MaHoaDonBan { get; set; }    
        public DateTime NgayHoaDon { get; set; } 
        public string PhuongThucThanhToan { get; set; } 
        public decimal TongTienHd { get; set; }  
        public List<ProductDetailViewModel> ChiTietSanPhams { get; set; } 
    }
}
