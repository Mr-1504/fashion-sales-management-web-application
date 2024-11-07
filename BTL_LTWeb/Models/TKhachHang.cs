using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models;

public partial class TKhachHang
{
    [Display(Name = "Mã khách hàng")]
    public int MaKhachHang { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Không đúng định giạng")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Tên khách hàng không được để trống")]
    [Display(Name = "Tên khách hàng")]
    public string? TenKhachHang { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateOnly? NgaySinh { get; set; }

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [ValidateNever]
    public virtual TUser User { get; set; } = null!;


}
