namespace BTL_LTWeb.ViewModels
{
    public class DiscountBannerViewModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
