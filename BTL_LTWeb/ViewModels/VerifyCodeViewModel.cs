using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.ViewModels
{
    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "Mã xác nhận không được để trống.")]
        public string ConfirmationCode { get; set; }
        public string Name { get; set; } = null!;
        public int Status { get; set; }
        public string Email { get; set; } = null!;
    }
}
