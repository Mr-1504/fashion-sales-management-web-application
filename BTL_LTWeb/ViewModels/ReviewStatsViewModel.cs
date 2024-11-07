using BTL_LTWeb.Models;

namespace BTL_LTWeb.ViewModels
{
    public class ReviewStatsViewModel
    {
        public List<int> starsCount { get; private set; } = new List<int>(new int[5]);
        public double score { get; private set; }

        public ReviewStatsViewModel(List<TDanhGia> reviews)
        {
            foreach (var it in reviews)
            {
                starsCount[it.Diem - 1]++;
            }
            score = 1.0 * (starsCount[0] + starsCount[1] * 2 + starsCount[2] * 3 + starsCount[3] * 4 + starsCount[4] * 5) / starsCount.Sum();
            score = double.Round(score, 2);
        }
    }
}
