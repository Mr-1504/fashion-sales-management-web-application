using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.ViewModels
{
    public class PaymentViewModel
    {
        public int MaKhachHang { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc.")]
        public string PhuongThucThanhToan { get; set; }

        
        public string HoTen { get; set; }

        public string DiaChi { get; set; }
        public string ThanhPho { get; set; }
        public string QuanHuyen { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(\d{10,11})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string SDT { get; set; }

        public string GhiChu { get; set; }

        // Thông tin giao hàng ở địa chỉ khác
        public int GiaoHangDiaChiKhac { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string DiaChiKhac { get; set; }
        [Required(ErrorMessage = "Thành phố là bắt buộc.")]
        public string ThanhPhoKhac { get; set; }
        [Required(ErrorMessage = "Quận/Huyện là bắt buộc.")]
        public string QuanHuyenKhac { get; set; }


        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(\d{10,11})$", ErrorMessage = "Số điện thoại khác không hợp lệ.")]
        public string SDTKhac { get; set; }
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public string HoTenKhac { get; set; }

        public List<int> CartID { get; set; }

        public List<string> AvailableDiscountCodes { get; set; }
        public string SelectedDiscountCode { get; set; }

    }

}
