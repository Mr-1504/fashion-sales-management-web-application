using System.ComponentModel;

namespace BTL_LTWeb.Models
{
    public class TChiTietHoaDonBan
    {
        [DisplayName("Mã hóa đơn bán")]
        public int MaHoaDonBan { get; set; }

        [DisplayName("Mã chi tiết sản phẩm")]
        public int MaChiTietSP { get; set; }

        [DisplayName("Đơn giá bán")]
        public decimal? DonGiaBan { get; set; }

        [DisplayName("Số lượng bán")]
        public int? SoLuongBan { get; set; }

        public TDanhMucSp DanhMucSP { get; set; }
        public THoaDonBan HoaDonBan { get; set; }
    }
}
