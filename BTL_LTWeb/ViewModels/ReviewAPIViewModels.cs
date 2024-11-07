using BTL_LTWeb.Models;

namespace BTL_LTWeb.ViewModels
{
    public class ReviewAPIGeneralStats
    {
        public string TenSP { get; set; }
        public double TongDiem { get; set; }
        public int SoLuotDanhGia { get; set; } = 0;

        public ReviewAPIGeneralStats(string tenSP, ReviewStatsViewModel stats)
        {
            TenSP = tenSP;
            TongDiem = stats.score > 0 ? stats.score : 0;
            SoLuotDanhGia += stats.starsCount.Sum();
        }
    }

    public class ReviewAPIIndividualReview
    {
        public string TenKhachHang { get; set; }
        public string ThoiGian { get; set; }
        public int DiemDanhGia { get; set; }
        public string? NoiDung { get; set; }
        public int? LuotThich { get; set; }
        public int? LuotKhongThich { get; set; }

        public ReviewAPIIndividualReview(ReviewContentViewModel review)
        {
            TenKhachHang = review.CsName;
            ThoiGian = review.DatePosted.ToString();
            DiemDanhGia = review.StarRated;
            if (!string.IsNullOrWhiteSpace(review.RvMessage)) 
            {
                NoiDung = review.RvMessage;
                LuotThich = review.VotesCasted.Where(it => it.Thich > 0).Count();
                LuotKhongThich = review.VotesCasted.Where(it => it.Thich < 0).Count();
            }
        }
    }

    public class ReviewAPIGet
    {
        public ReviewAPIGeneralStats statistical { get; private set; }
        public List<ReviewAPIIndividualReview> reviews { get; private set; }
        public ReviewAPIGet(ReviewAPIGeneralStats statistical, List<ReviewAPIIndividualReview> reviews)
        {
            this.statistical = statistical;
            this.reviews = reviews;
        }
    }
}
