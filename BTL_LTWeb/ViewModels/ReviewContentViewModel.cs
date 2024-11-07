using BTL_LTWeb.Models;

namespace BTL_LTWeb.ViewModels
{
    public class ReviewHistory
    {
        public DateTime DatePosted { get; set; }
        public int StarRated { get; set; }
        public string Message { get; set; }
        public int LikeCount { get; set; }
        public int HateCount { get; set; }
    }

    public class ReviewContentViewModel
    {
        public int _reviewID { get; set; }
        public int _productID { get; set; }
        public int _userID { get; set; }
        public int? _emplID { get; set; }
        public string CsName { get; set; }
        public DateTime DatePosted { get; set; }
        public int StarRated { get; set; }
        public string? RvMessage { get; set; }
        public string? EpName { get; set; }
        public string? RpMessage { get; set; }
        public List<TPhanHoi> VotesCasted { get; set; } = new List<TPhanHoi>();
        public List<ReviewHistory> OldReviews { get; set; } = new List<ReviewHistory>();
    }
}
