
using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models
{
    public class TGiaoHang
    {

        public int MaGiaoHang { get; set; }
        public int MaHoaDonBan { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Thành Phố")]
        public string ThanhPho { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public string QuanHuyen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string HoTenNguoiNhan { get; set; }


        public virtual THoaDonBan HoaDonBan { get; set; }
    }
    
}
