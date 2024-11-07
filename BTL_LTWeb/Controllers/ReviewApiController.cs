using BTL_LTWeb.Models;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BTL_LTWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewApiController : Controller
    {
        private readonly QLBanDoThoiTrangContext _db;

        public ReviewApiController(QLBanDoThoiTrangContext _context)
        {
            _db = _context;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            HashSet<int> MaSPs = _db.TDanhGias.Select(it => it.MaSP).ToHashSet();
            List<ReviewAPIGet> res = new List<ReviewAPIGet>();
            foreach(int itSP in MaSPs)
            {
                List<TDanhGia> reviews = _db.TDanhGias.Where(it => it.MaSP == itSP).ToList();
                var query = (from RV in reviews
                             join KH in _db.TKhachHangs on RV.MaKhachHang equals KH.MaKhachHang
                             select new ReviewContentViewModel
                             {
                                 CsName = KH.TenKhachHang != null ? KH.TenKhachHang : KH.Email,
                                 DatePosted = RV.NgayTao,
                                 StarRated = RV.Diem,
                                 RvMessage = RV.BinhLuan
                             }).Where(it => !string.IsNullOrEmpty(it.RvMessage)).ToList();
                foreach (var it in query)
                {
                    List<TPhanHoi> qry = _db.TPhanHois.Where(t => it._reviewID == t.MaDanhGia).ToList();
                    it.VotesCasted = qry;
                }
                List<ReviewAPIIndividualReview> apiReviews = new List<ReviewAPIIndividualReview>();
                foreach (var it in query) apiReviews.Add(new ReviewAPIIndividualReview(it));
                string? tsp = _db.TDanhMucSps.FirstOrDefault(it => it.MaSp == itSP)?.TenSp;
                res.Add(new ReviewAPIGet(new ReviewAPIGeneralStats(string.IsNullOrWhiteSpace(tsp) ? "noname" : tsp, new ReviewStatsViewModel(reviews)), apiReviews));
            }
            if (res.Count > 0) return Ok(res); else return NoContent();
        }

        [HttpGet("getproduct")]
        public IActionResult GetReviewOfProduct(int pid)
        {
            List<TDanhGia> reviews = _db.TDanhGias.Where(it => it.MaSP == pid).ToList();
            var query = (from RV in reviews
                        join KH in _db.TKhachHangs on RV.MaKhachHang equals KH.MaKhachHang
                        select new ReviewContentViewModel
                        {
                            CsName = KH.TenKhachHang != null ? KH.TenKhachHang : KH.Email,
                            DatePosted = RV.NgayTao,
                            StarRated = RV.Diem,
                            RvMessage = RV.BinhLuan
                        }).Where(it => !string.IsNullOrEmpty(it.RvMessage)).ToList();
            foreach (var it in query)
            {
                List<TPhanHoi> qry = _db.TPhanHois.Where(t => it._reviewID == t.MaDanhGia).ToList();
                it.VotesCasted = qry;
            }
            List<ReviewAPIIndividualReview> apiReviews = new List<ReviewAPIIndividualReview>();
            foreach (var it in query) apiReviews.Add(new ReviewAPIIndividualReview(it));
            string? tsp = _db.TDanhMucSps.FirstOrDefault(it => it.MaSp == pid)?.TenSp;
            ReviewAPIGet res = (new ReviewAPIGet(new ReviewAPIGeneralStats(string.IsNullOrWhiteSpace(tsp) ? "noname" : tsp, new ReviewStatsViewModel(reviews)), apiReviews));
            if (res.reviews.Count > 0) return Ok(res); else return NoContent();
        }
    }
}
