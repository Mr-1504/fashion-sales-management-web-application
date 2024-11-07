using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models;

public partial class TNhanVien
{
    public int MaNhanVien { get; set; }

    [Required(ErrorMessage = "Email không được để trống")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Tên nhân viên không được để trống")]
    [MinLength(4,ErrorMessage = "Tên nhân viên phải có ít nhất 4 ký tự")]
    [MaxLength(100, ErrorMessage = "Tên nhân viên không được vượt quá 100 ký tự")]
    [Display(Name = "Tên nhân viên")]
    public string? TenNhanVien { get; set; }

    [Range(typeof(DateTime), "1/1/1960", "31/12/2004")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [Required(ErrorMessage = "Chức vụ không được để trống")]
    [Display(Name = "Chức vụ")]
    public string? ChucVu { get; set; }

    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [ValidateNever]
    public virtual TUser UsernameNavigation { get; set; } = null!;


}
