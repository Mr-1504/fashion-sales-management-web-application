using BTL_LTWeb.Models;

namespace BTL_LTWeb.ViewModels
{
    public class HomeProductDetailViewModel
    {
        public TDanhMucSp product { get; set; }
        public List<TAnhSp> productImages { get; set; }
        public List<string> Sizes { get; set; }             
        public List<string> Colors { get; set; }
    }
}
