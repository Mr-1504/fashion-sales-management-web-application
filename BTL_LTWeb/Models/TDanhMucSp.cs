using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models;

public partial class TDanhMucSp
{
    public int MaSp { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [DisplayName("Tên sản phẩm")]
    public string? TenSp { get; set; }

    [Required(ErrorMessage = "Chất liệu không được để trống")]
    [DisplayName("Chất liệu")]
    public string? ChatLieu { get; set; }

    [Required(ErrorMessage = "Loại không được để trống")]
    [DisplayName("Loại")]
    public string? LoaiDt { get; set; }

    [Required(ErrorMessage = "Hãng sản xuất không được để trống")]
    [DisplayName("Hãng sản xuất")]
    public string? HangSx { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Thời gian bảo hành phải là số không âm")]
    [DisplayName("Thời gian bảo hành")]
    public int? ThoiGianBaoHanh { get; set; }

    [DisplayName("Giới thiệu sản phẩm")]
    public string? GioiThieuSp { get; set; }

    [DisplayName("Ảnh đại diện")]
    public string? AnhDaiDien { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải là số không âm")]
    [DisplayName("Giá")]
    public decimal? Gia { get; set; }

    public int TagId { get; set; }
    [ValidateNever]
    public virtual TTag Tag { get; set; } = new TTag();
    [ValidateNever]
    public virtual ICollection<TAnhSp> TAnhSps { get; set; } = new List<TAnhSp>();
    [ValidateNever]
    public virtual ICollection<TChiTietSanPham> TChiTietSanPhams { get; set; } = new List<TChiTietSanPham>();


}
