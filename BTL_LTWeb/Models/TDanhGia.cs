using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models
{
    public class TDanhGia
    {
        public int MaDanhGia { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [Required]
        public int MaSP { get; set; }

        [Required]
        public DateTime NgayTao { get; set; }

        [Required]
        public int Diem { get; set; }

        [MaxLength(1000)]
        public string? BinhLuan { get; set; }
        public string? LichSu { get; set; }
        public int? MaNhanVien { get; set; }

        [MaxLength(1000)]
        public string? TraLoi { get; set; }
    }
}
