using Microsoft.AspNetCore.Mvc;
using BTL_LTWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace BTL_LTWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/TTags")]
    [Authorize(Roles = "Admin,NhanVien")]
    public class TTagsController : Controller
    {
        private readonly QLBanDoThoiTrangContext _context;
        private readonly int pageSize = 7;
        public TTagsController(QLBanDoThoiTrangContext qLBanDoThoiTrangContext)
        {
            _context = qLBanDoThoiTrangContext;
        }

        public IActionResult Index()
        {
            var shops = _context.Tags.AsQueryable();
            int pageNum = (int)Math.Ceiling(shops.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            ViewBag.keyword = "";
            var result = shops.Take(pageSize).ToList();
            return View(result);
        }

        [HttpGet]
        [Route("TagFilter")]
        public IActionResult TagFilter(string? keyword, int? pageIndex)
        {
            var shops = _context.Tags.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                shops = shops.Where(l => l.TagName.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword;
            }
            int page = (pageIndex ?? 1);
            int pageNum = (int)Math.Ceiling(shops.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum;
            var result = shops.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            return PartialView("TagTable", result);
        }
        [Route("Details")]
        public IActionResult Details(int? Tagid)
        {
            if (Tagid == null)
            {
                return NotFound();
            }

            var tTag = _context.Tags
                .FirstOrDefault(m => m.TagId == Tagid);
            if (tTag == null)
            {
                return NotFound();
            }

            return View(tTag);
        }


        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("TagId,TagName,TagImage")] TTag tg, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {

                    string targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesTag");
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string targetFilePath = Path.Combine(targetDirectory, file.FileName);


                    using (var stream = new FileStream(targetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }


                    tg.TagImage = file.FileName;
                }

                _context.Tags.Add(tg);
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            return View(tg);
        }

        // GET: Admin/TTags/Edit/5
        [Route("Edit")]
        public IActionResult Edit(int? Tagid)
        {
            if (Tagid == null)
            {
                return NotFound();
            }

            var tTag = _context.Tags.Find(Tagid);
            if (tTag == null)
            {
                return NotFound();
            }
            return View(tTag);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Tagid, [Bind("TagId,TagName,TagImage")] TTag tTag, IFormFile file)
        {
            if (Tagid != tTag.TagId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                if (file != null && file.Length > 0)
                {
                    string targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesTag");
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    string targetFilePath = Path.Combine(targetDirectory, file.FileName);


                    using (var stream = new FileStream(targetFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    tTag.TagImage = file.FileName;
                }

                _context.Tags.Update(tTag);
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return View(tTag);
        }
        [Route("Delete")]
        public IActionResult Delete(int? Tagid)
        {
            if (Tagid == null)
            {
                return NotFound();
            }

            var tTag = _context.Tags
                .FirstOrDefault(m => m.TagId == Tagid);
            if (tTag == null)
            {
                return NotFound();
            }

            return View(tTag);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete")]
        public IActionResult DeleteConfirmed(int Tagid)
        {
            var tTag = _context.Tags.Find(Tagid);
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesTag", tTag.TagImage);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            
            if (tTag != null)
            {
                _context.Tags.Remove(tTag);
            }
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool TTagExists(int id)
        {
            return _context.Tags.Any(e => e.TagId == id);
        }



    }
}
