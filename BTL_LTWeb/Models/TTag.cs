using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BTL_LTWeb.Models
{
    public class TTag
    {
        
        public int TagId { get; set; }
        [DisplayName("Tên Bộ sưu tập")]
        [Required(ErrorMessage = "Tên bộ sưu tập không được để trống")]
        public string TagName { get; set; }
        [DisplayName("Ảnh bộ sưu tập")]
        public string? TagImage { get; set; }
    }
}
