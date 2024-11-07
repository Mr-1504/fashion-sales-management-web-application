using Humanizer;
using Microsoft.CodeAnalysis.Options;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;

namespace BTL_LTWeb.Models
{
    public partial class TDanhSachCuaHang
    {
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDTCuaHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập khu vực")]
        public string KhuVuc { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập kinh độ")]
        [Range(-180, 180, ErrorMessage = "Kinh độ không hợp lệ")]
        public float KinhDo { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vĩ độ")]
        [Range(-90, 90, ErrorMessage = "Vĩ độ không hợp lệ")]
        public float ViDo { get; set; }

    }
}

