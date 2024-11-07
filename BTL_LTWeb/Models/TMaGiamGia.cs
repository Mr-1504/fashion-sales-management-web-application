using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BTL_LTWeb.Models
{
    public partial class TMaGiamGia
    {
        public int MaGiamGia { get; set; }
        [Required(ErrorMessage = "Code không được để trống")]
        [StringLength(8, ErrorMessage = "Mã giảm giá không được vượt quá 8 ký tự")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Tỉ lệ giảm không được để trống")]
        [Display(Name = "Tỉ lệ giảm")]
        [Range(1, 100, ErrorMessage = "Giá trị giảm phải từ 1 đến 100")]
        public decimal TiLeGiam { get; set; }

        [Display(Name = "Mô tả")]
        public string? Mota {  get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime? NgayKetThuc {  get; set; }
        public int? TrangThai { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayBatDau.HasValue && NgayKetThuc.HasValue)
            {
                if (NgayKetThuc < NgayBatDau)
                {
                    yield return new ValidationResult(
                        "Ngày kết thúc không thể trước ngày bắt đầu.",
                        new[] { nameof(NgayKetThuc) });
                }
            }
        }

    }
}
