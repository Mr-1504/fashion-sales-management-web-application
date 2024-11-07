using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models;

public partial class THoaDonBan
{
    [Display(Name = "Mã hóa đơn bán")]
    public int MaHoaDonBan { get; set; }

    [Display(Name = "Ngày hóa đơn")]
    public DateTime? NgayHoaDon { get; set; }

    [Display(Name = "Mã khách hàng")]
    public int? MaKhachHang { get; set; }

    [Display(Name = "Tổng tiền hóa đơn")]
    public decimal? TongTienHd { get; set; }

    [Display(Name = "Phương thức thanh toán")]
    public string? PhuongThucThanhToan { get; set; }

    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [Display(Name = "Mã giảm giá")]
    public int? MaGiamGia { get; set; }

    public virtual TKhachHang? KhachHang { get; set; }
    public virtual TMaGiamGia? GiamGia { get; set; }
    public virtual TGiaoHang GiaoHang { get; set; }
    public virtual ICollection<TChiTietHoaDonBan> TChiTietHoaDonBans { get; set; } = new List<TChiTietHoaDonBan>();
}
