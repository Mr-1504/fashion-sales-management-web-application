using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [RegularExpression(@"^[a-zA-ZÀÁẢÃẠÂẦẤẨẪẬĂẰẮẲẴẶÈÉẺẼẸÊỀẾỂỄỆĐÌÍỈĨỊÒÓỎÕỌÔỒỐỔỖỘƠỜỚỞỠỢÙÚỦŨỤƯỪỨỬỮỰỲỴỶỸÝàáảãạâầấẩẫậăằắẳẵặèéẻẽẹêềếểễệđìíỉĩịòóỏõọôồốổỗộơờớởỡợùúủũụưừứửữựỳỵỷỹý -]+$", ErrorMessage = "Tên không hợp lệ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [CustomDateValidation(ErrorMessage = "Ngày sinh không hợp lệ.")]
        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Chọn đủ thông tin")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Chọn đủ thông tin")]
        public string District { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[35789][0-9]{8}|84[35789][0-9]{8}|\+84[35789][0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 8)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$",
        ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường, 1 số, và 1 ký tự đặc biệt.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        public string ConfirmPassword { get; set; }
    }

    public class CustomDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly dateValue)
            {
                // Kiểm tra ngày sinh có phải là ngày trong quá khứ không
                if (dateValue > DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ValidationResult("Ngày sinh phải là một ngày trong quá khứ.");
                }

                // Kiểm tra định dạng của ngày
                var day = dateValue.Day;
                var month = dateValue.Month;
                var year = dateValue.Year;

                // Kiểm tra số ngày trong tháng
                if (day > DateTime.DaysInMonth(year, month))
                {
                    return new ValidationResult("Ngày không hợp lệ cho tháng này.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
