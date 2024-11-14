using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOANMOI.Models;
using PagedList;
using PagedList.Mvc;
namespace DOANMOI.Controllers
{
    public class TrangChuController : Controller
    {
        private TapChi1Entities db = new TapChi1Entities();

        private bool IsAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }
        // GET: TrangChu
        public ActionResult Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            var bai_viet = db.Bai_viet.Include(b => b.Danh_Muc).Include(b => b.User);
            return View(bai_viet.ToList());
        }

        // GET: TrangChu/Details/5
        public ActionResult Details(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bai_viet bai_viet = db.Bai_viet.Find(id);
            if (bai_viet == null)
            {
                return HttpNotFound();
            }
            return View(bai_viet);
        }

        // GET: TrangChu/Create
        public ActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            ViewBag.DanhMucID = new SelectList(db.Danh_Muc, "DanhMucID", "TenDanhMuc");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName");
            return View();
        }

        // POST: TrangChu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BaiVietID,TieuDe,NoiDung,DanhMucID,Hinh_Anh,Thoi_Gian_Tao,UserID")] Bai_viet bai_viet)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (ModelState.IsValid)
            {
                db.Bai_viet.Add(bai_viet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DanhMucID = new SelectList(db.Danh_Muc, "DanhMucID", "TenDanhMuc", bai_viet.DanhMucID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", bai_viet.UserID);
            return View(bai_viet);
        }

        // GET: TrangChu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bai_viet bai_viet = db.Bai_viet.Find(id);
            if (bai_viet == null)
            {
                return HttpNotFound();
            }
            ViewBag.DanhMucID = new SelectList(db.Danh_Muc, "DanhMucID", "TenDanhMuc", bai_viet.DanhMucID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", bai_viet.UserID);
            return View(bai_viet);
        }

        // POST: TrangChu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BaiVietID,TieuDe,NoiDung,DanhMucID,Hinh_Anh,Thoi_Gian_Tao,UserID")] Bai_viet bai_viet)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (ModelState.IsValid)
            {
                db.Entry(bai_viet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DanhMucID = new SelectList(db.Danh_Muc, "DanhMucID", "TenDanhMuc", bai_viet.DanhMucID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", bai_viet.UserID);
            return View(bai_viet);
        }

        // GET: TrangChu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bai_viet bai_viet = db.Bai_viet.Find(id);
            if (bai_viet == null)
            {
                return HttpNotFound();
            }
            return View(bai_viet);
        }

        // POST: TrangChu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            Bai_viet bai_viet = db.Bai_viet.Find(id);
            db.Bai_viet.Remove(bai_viet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult home(string danhmuc,string SearchString, int? page)
        {
            var bai_viet = db.Bai_viet.Include(b => b.Danh_Muc).Include(b => b.User);
            // Lọc theo danh mục nếu danh mục không rỗng
            if (!string.IsNullOrEmpty(danhmuc))
            {
                int danhMucID;
                if (int.TryParse(danhmuc, out danhMucID))
                {
                    bai_viet = bai_viet.Where(b => b.Danh_Muc.DanhMucID == danhMucID);
                }
            }
            // Kiểm tra xem có chuỗi tìm kiếm không
            if (!String.IsNullOrEmpty(SearchString))
            {
                bai_viet = bai_viet.Where(s => s.TieuDe.Contains(SearchString) ||
                                                s.NoiDung.Contains(SearchString) ||
                                                s.Danh_Muc.TenDanhMuc.Contains(SearchString));
            }
            // Nếu SearchString không rỗng và không có kết quả trả về ViewBag
            if (!String.IsNullOrEmpty(SearchString) && !bai_viet.Any())
            {
                ViewBag.Message = "Không tìm thấy ";
            }
            // Khai báo mỗi trang 4 sản phẩm 
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            // Sắp xếp dữ liệu theo thời gian tạo
            bai_viet = bai_viet.OrderBy(b => b.Thoi_Gian_Tao);

            // Trả về các bài viết được phân trang
            return View(bai_viet.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult FASHION(string SearchString, int? page)
        {
            return home("1", SearchString, page);
        }
        public ActionResult CUISINE(string SearchString, int? page)
        {
            return home("2", SearchString, page);
        }
        public ActionResult ENTERTAIMENT(string SearchString, int? page)
        {
            return home("3", SearchString, page);
        }
        public ActionResult SPORT(string SearchString, int? page)
        {
            return home("4", SearchString, page);
        }
        public ActionResult ThongBao()
        {
            return View();
        }
    }
}
