using BTL_LTWeb.Models;
using BTL_LTWeb.Services;
using BTL_LTWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing.Text;
using System.Security.Claims;
using System.Text.Json;

namespace BTL_LTWeb.Controllers
{
    public class ReviewController : Controller
    {
        private QLBanDoThoiTrangContext _db;
        private List<TDanhGia> _reviews = new List<TDanhGia>();
        private List<TPhanHoi> _reacts = new List<TPhanHoi>();

        //common account variable
        private int _uid;
        private string _utype = string.Empty;

        //currently in product's page
        private int _pid;

        //for pagination purpose
        private const int _PERPAGERV = 5;

        //for email service
        private readonly EmailService _emailService;

        public ReviewController(QLBanDoThoiTrangContext _context, EmailService emailService)
        {
            _db = _context;
            _emailService = emailService;
        }

        private void FetchCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                string email = User.FindFirst(ClaimTypes.Email)?.Value!;
                if (!string.IsNullOrWhiteSpace(email)) //đã đăng nhập
                {
                    var user = _db.TUsers.FirstOrDefault(e => e.Email == email);
                    _utype = user.LoaiUser;
                    switch (_utype)
                    {
                        case "KhachHang":
                            _uid = _db.TKhachHangs.Where(it => it.Email == email).First().MaKhachHang;
                            break;
                        case "NhanVien":
                            _uid = _db.TNhanViens.Where(it => it.Email == email).First().MaNhanVien;
                            break;
                        default:
                            _uid = 0;
                            break;
                    }
                }
            }
            Console.WriteLine(">> " + _uid + " | " + _utype);
        }

        private void FetchProduct(int pid)
        {
            if (_db.TDanhMucSps.Any(it => it.MaSp == pid)) //trong trường hợp mã sp không hợp lệ
            {
                _pid = pid;
                _reviews = _db.TDanhGias.Where(it => it.MaSP == _pid).ToList();
                List<int> rids = new List<int>();
                foreach (var it in _reviews) rids.Add(it.MaDanhGia);
                _reacts = _db.TPhanHois.Where(it => rids.Contains(it.MaDanhGia)).ToList();
            }
            else
            {
                _pid = 0;
                _reviews = new List<TDanhGia>();
                _reacts = new List<TPhanHoi>();
            }
        }

        private bool IsUserPurchasedProduct()
        {
            if (_uid < 1 || _pid < 1) return false;
            var dsChiTiet = _db.TChiTietSanPhams.Where(it => it.MaSp == _pid).Select(it => it.MaChiTietSp).ToList();
            var dsHoaDon = _db.THoaDonBans.Where(it => it.MaKhachHang == _uid).Select(it => it.MaHoaDonBan).ToList();
            return _db.TChiTietHoaDonBans.Any(it => dsHoaDon.Contains(it.MaHoaDonBan) && dsChiTiet.Contains(it.MaChiTietSP));
        }

        public IActionResult GlobalReviewAJAX(int pid, string email)
        {
            FetchCurrentUser();
            ViewBag.utype = _utype;
            ViewBag.pid = pid;

            return PartialView("globalRV");
        }
        private IEnumerable<ReviewContentViewModel> QueryDisplableReviews()
        {
            var query = (from RV in _db.TDanhGias
                         join KH in _db.TKhachHangs on RV.MaKhachHang equals KH.MaKhachHang
                         join NV in _db.TNhanViens on RV.MaNhanVien equals NV.MaNhanVien into gj
                         from subNV in gj.DefaultIfEmpty()
                         select new ReviewContentViewModel
                         {
                             _reviewID = RV.MaDanhGia,
                             _productID = RV.MaSP,
                             _userID = RV.MaKhachHang,
                             _emplID = RV.MaNhanVien,
                             CsName = KH.TenKhachHang != null ? KH.TenKhachHang : KH.Email,
                             DatePosted = RV.NgayTao,
                             StarRated = RV.Diem,
                             RvMessage = RV.BinhLuan,
                             EpName = subNV.Email == null ? null : subNV.TenNhanVien == null ? subNV.Email : subNV.TenNhanVien,
                             RpMessage = RV.TraLoi
                         }).Where(it => !string.IsNullOrEmpty(it.RvMessage) && it._productID == _pid).ToList();
            foreach (var it in query)
            {
                List<TPhanHoi> qry = _reacts.Where(t => it._reviewID == t.MaDanhGia).ToList();
                it.VotesCasted = qry;
                string? hsString = _reviews.FirstOrDefault(t => t.MaDanhGia == it._reviewID)?.LichSu;
                if (!string.IsNullOrWhiteSpace(hsString)) it.OldReviews = JsonSerializer.Deserialize<List<ReviewHistory>>(hsString)!;
            }
            return query.Where(it => !string.IsNullOrEmpty(it.RvMessage) && it._productID == _pid).ToList();
        }

        public IActionResult GetStatsPV(int pid)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            return PartialView("rvStats", new ReviewStatsViewModel(_reviews));
        }
        public IActionResult GetMakerPV(int pid)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            TDanhGia userReview =
                _utype == "KhachHang" ? _reviews.Find(it => it.MaKhachHang == _uid)! : new TDanhGia();

            ViewBag.pid = _pid;
            ViewBag.hasPurchased = IsUserPurchasedProduct();

            return PartialView("rvMaker", userReview == null ? new TDanhGia() : userReview);
        }
        public IActionResult GetListPV(int pid, string sortType, int pageNum)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            ViewBag.utype = _utype;
            ViewBag.uid = _uid;
            ViewBag.pid = _pid;

            IOrderedEnumerable<ReviewContentViewModel> dprvlist;
            switch (sortType)
            {
                case "sort_Stars":
                    dprvlist = QueryDisplableReviews().OrderByDescending(it => it.StarRated);
                    break;
                case "sort_Date":
                    dprvlist = QueryDisplableReviews().OrderByDescending(it => it.DatePosted);
                    break;
                case "sort_Helpful":
                    dprvlist = QueryDisplableReviews().OrderByDescending(it => it.VotesCasted.Where(it => it.HuuIch > 0).Count());
                    break;
                default:
                    dprvlist = QueryDisplableReviews().OrderByDescending(it => it.StarRated);
                    break;
            }

            ViewBag.pageCount = dprvlist.Count() / _PERPAGERV;
            if (dprvlist.Count() % _PERPAGERV > 0) ViewBag.pageCount++;

            //prevent oob values
            if (pageNum > ViewBag.pageCount) pageNum = ViewBag.pageCount;
            else if (pageNum < 1) pageNum = 1;

            //remind the view which page it has called for
            ViewBag.currentPage = pageNum == 0 ? 1 : pageNum;

            return PartialView("rvList", dprvlist.Skip((pageNum - 1) * _PERPAGERV).Take(_PERPAGERV));
        }

        //successor of AuthorityCheck
        private string ValidatingAction(string action, TDanhGia affected, TDanhGia? prev = null)
        {
            /*
                x SACT    action run successfull
                WUNS    user hasnt bought product yet
                WRFP    point rating wrong format (blank)
                WRFC    review message wrong format (>1000c)
                WRNE    new review doesnt change at all
                WRNC    reply message blank
                EUNP    logged in user lack in permission
                x ERNI    rid not found in database
                x EDTB    error in database (unexpected)
             */
            switch (action)
            {
                case "rvCreate":
                    if (_utype != "KhachHang") return "EUNP";
                    if (!IsUserPurchasedProduct()) return "WUNS";
                    if (affected.Diem < 1 || affected.Diem > 5) return "WRFP";
                    if (!string.IsNullOrWhiteSpace(affected.BinhLuan) && affected.BinhLuan.Length > 1000) return "WRFC";
                    break;
                case "rvEdit":
                    if (_utype != "KhachHang" || _uid != prev.MaKhachHang) return "EUNP";
                    if (affected.Diem < 1 || affected.Diem > 5) return "WRFP";
                    if (!string.IsNullOrWhiteSpace(affected.BinhLuan) && affected.BinhLuan.Length > 1000) return "WRFC";
                    if (affected.Diem == prev.Diem && affected.BinhLuan == prev.BinhLuan) return "WRNE";
                    break;
                case "rvDelete":
                    if (_utype != "KhachHang" || _uid != affected.MaKhachHang) return "EUNP";
                    break;
                case "rpCreate":
                    if (_utype == "KhachHang") return "EUNP";
                    if (string.IsNullOrWhiteSpace(affected.TraLoi)) return "WRNC";
                    if (affected.TraLoi.Length > 1000) return "WRFC";
                    break;
                case "rpEdit":
                    if (_utype == "KhachHang" || _uid != prev.MaNhanVien) return "EUNP";
                    if (string.IsNullOrWhiteSpace(affected.TraLoi)) return "WRNC";
                    if (affected.TraLoi.Length > 1000) return "WRFC";
                    break;
                case "rpDelete":
                    if (_utype == "KhachHang" || _uid != affected.MaNhanVien) return "EUNP";
                    break;
                case "urCastVote":
                    if (_utype != "KhachHang") return "EUNP";
                    break;
                case "dbError":
                    return "EDTB";
            }
            return "SACT";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string AlterReview([Bind("Diem,BinhLuan")] TDanhGia inpReview, int pid)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            TDanhGia userReview = _reviews.Find(it => it.MaKhachHang == _uid)!;

            //khách chưa để lại bình luận -> tạo bình luận
            if (userReview == null)
            {
                inpReview.MaKhachHang = _uid;
                inpReview.MaSP = _pid;
                inpReview.NgayTao = DateTime.Now;

                string actStatus = ValidatingAction("rvCreate", inpReview);
                if (actStatus != "SACT") return actStatus;

                if (ModelState.IsValid) try
                    {
                        _db.TDanhGias.Add(inpReview);
                        _db.SaveChanges();
                    }
                    catch
                    {
                        return ValidatingAction("dbError", inpReview);
                    }
            }
            //khách đã để lại bình luận -> sửa bình luận
            else
            {
                string actStatus = ValidatingAction("rvEdit", inpReview, userReview);
                if (actStatus != "SACT") return actStatus;

                //Bình luận sửa cách nhau 12 tiếng -> lưu vào LichSu
                if (DateTime.Now.Subtract(userReview.NgayTao).TotalHours > 3 && !string.IsNullOrWhiteSpace(userReview.BinhLuan))
                {
                    List<ReviewHistory> rvHistories = new List<ReviewHistory>();
                    if (!string.IsNullOrWhiteSpace(userReview.LichSu)) rvHistories = JsonSerializer.Deserialize<List<ReviewHistory>>(userReview.LichSu)!;
                    ReviewHistory recentHistory = new();
                        recentHistory.DatePosted = userReview.NgayTao;
                        recentHistory.StarRated = userReview.Diem;
                        recentHistory.Message = userReview.BinhLuan;
                        var reactList = _db.TPhanHois.Where(it => it.MaDanhGia == userReview.MaDanhGia);
                        recentHistory.LikeCount = reactList.Where(it => it.Thich > 0).Count();
                        recentHistory.HateCount = reactList.Where(it => it.Thich < 0).Count();
                    rvHistories.Add(recentHistory);
                    userReview.LichSu = JsonSerializer.Serialize(rvHistories);
                }

                userReview.NgayTao = DateTime.Now;
                userReview.Diem = inpReview.Diem;
                userReview.BinhLuan = inpReview.BinhLuan;

                return EditReview(userReview);
            }
            return "SACT";
        }

        public IActionResult DeleteReviewAsk(int pid, int rid)
        {
            ViewBag.rid = rid;
            ViewBag.pid = pid;
            return PartialView("rvDeleteConfirm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string DeleteReviewConfirm(int pid, int rid)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            var review = _reviews.FirstOrDefault(it => it.MaDanhGia == rid);
            if (review != null)
            {
                string actStatus = ValidatingAction("rvDelete", review);
                if (actStatus != "SACT") return actStatus;

                //xoá review -> xoá react của review
                if (_reacts.Any(it => it.MaDanhGia == rid))
                    try
                    {
                        foreach (var it in _reacts.Where(it => it.MaDanhGia == rid)) _db.TPhanHois.Remove(it);
                        _db.SaveChanges();
                    }
                    catch
                    {
                        return "EDTB";
                    }
                try
                {
                    _db.TDanhGias.Remove(review);
                    _db.SaveChanges();
                }
                catch
                {
                    return "EDTB";
                }
            }
            else return "ERNI";
            return "SACT";
        }

        private string EditReview(TDanhGia rv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.TDanhGias.Update(rv);
                    _db.SaveChanges();
                    return "SACT";
                }
                catch
                {
                    return "EDTB";
                }
            }
            return "EDTB";
        }

        public IActionResult OpenReplySection(int pid, int rid)
        {
            ViewBag.rid = rid;
            ViewBag.pid = pid;
            TDanhGia queryRv = _db.TDanhGias.Find(rid)!;
            if (string.IsNullOrWhiteSpace(queryRv.TraLoi)) return PartialView("rvReplyEdit"); else return PartialView("rvReplyEdit", queryRv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string AlterReply([Bind("MaDanhGia", "TraLoi")] TDanhGia inpReply, int pid)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            TDanhGia frameReview = _reviews.Find(it => it.MaDanhGia == inpReply.MaDanhGia)!;
            if (frameReview != null)
            {
                if (frameReview.MaNhanVien == null)
                {
                    string actStatus = ValidatingAction("rpCreate", inpReply);
                    if (actStatus != "SACT") return actStatus;

                    frameReview.MaNhanVien = _uid;
                    frameReview.TraLoi = inpReply.TraLoi;
                }
                else
                {
                    string actStatus = ValidatingAction("rpEdit", inpReply, frameReview);
                    if (actStatus != "SACT") return actStatus;

                    //sửa câu trả lời -> xoá chỉ số hữu ích cũ
                    foreach (var it in _reacts.Where(it => it.MaDanhGia == frameReview.MaDanhGia))
                    {
                        it.HuuIch = 0;
                        _db.TPhanHois.Update(it);
                    }
                    frameReview.TraLoi = inpReply.TraLoi;
                }
                if (EditReview(frameReview) == "SACT")
                {
                    string csEmail = _db.TKhachHangs.FirstOrDefault(it => it.MaKhachHang == frameReview.MaKhachHang).Email;
                    string emName = _db.TNhanViens.FirstOrDefault(it => it.MaNhanVien == frameReview.MaNhanVien).TenNhanVien;
                    string pdName = _db.TDanhMucSps.FirstOrDefault(it => it.MaSp == frameReview.MaSP).TenSp;
                    string rpMsg = frameReview.TraLoi;
                    SendNoticeOfReply(csEmail != null ? csEmail : "", emName != null ? emName : "", pdName != null ? pdName : "", rpMsg != null ? rpMsg : "");
                    return "SACT";
                }
            }
            return "ERNI";
        }

        public string DeleteReply(int pid, int rid)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            TDanhGia? frameReview = _reviews.Find(it => it.MaDanhGia == rid);
            if (frameReview != null)
            {
                string actStatus = ValidatingAction("rpDelete", frameReview);
                if (actStatus != "SACT") return actStatus;

                foreach (var it in _reacts.Where(it => it.MaDanhGia == frameReview.MaDanhGia))
                {
                    it.HuuIch = 0;
                    _db.TPhanHois.Update(it);
                }
                frameReview.MaNhanVien = null;
                frameReview.TraLoi = null;

                return EditReview(frameReview);
            }
            return "ERNI";
        }

        public string CastVote(int pid, int rid, char type)
        {
            FetchCurrentUser();
            FetchProduct(pid);

            var query = _reacts.Where(it => it.MaKhachHang == _uid && it.MaDanhGia == rid);
            TPhanHoi? castedVote = null;
            if (query.Count() > 0) castedVote = query.First();
            TDanhGia? voteatReview = _reviews.Find(it => it.MaDanhGia == rid);

            //không có review == không thể vote (=> data tempering)
            if (voteatReview == null) return "ERNI";

            string actStatus = ValidatingAction("urCastVote", new TDanhGia());
            if (actStatus != "SACT") return actStatus;

            //tạo vote entry cho khách khi db chưa có
            if (castedVote == null)
            {
                castedVote = new TPhanHoi();
                castedVote.MaDanhGia = rid;
                castedVote.MaKhachHang = _uid;
                castedVote.Thich = 0;
                castedVote.HuuIch = 0;
                try
                {
                    _db.TPhanHois.Add(castedVote);
                    _db.SaveChanges();
                }
                catch
                {
                    return "EDTB";
                }
            }
            //cast vote theo option (đè vote = cancel out)
            switch (type)
            {
                case 'L':
                    castedVote.Thich = castedVote.Thich == 1 ? 0 : 1;
                    break;
                case 'D':
                    castedVote.Thich = castedVote.Thich == -1 ? 0 : -1;
                    break;
                case 'H':
                    if (voteatReview.MaNhanVien != null) castedVote.HuuIch = castedVote.HuuIch == 1 ? 0 : 1;
                    break;
            }

            try
            {
                _db.TPhanHois.Update(castedVote);
                _db.SaveChanges();
            }
            catch { return "EDTB"; }
            return "SACT";
        }

        private void SendNoticeOfReply(string csEmail, string emName, string pdName, string rpMsg)
        {
            string reply =
                $"<p>Sản phẩm: <strong>{pdName}</strong></p>" +
                $"<p>Nhân viên: <strong>{emName}</strong></p>" +
                $"<p>Nội dung:</p>" +
                $"<p style='margin-left: 10px; border-left: solid 1px lightgray'>{string.Join("<br>", rpMsg.Split('\n'))}</p>";
            _emailService.SendEmailAsync(csEmail, emName, reply, 5);
        }
    }
}
