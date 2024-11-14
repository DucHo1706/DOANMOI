using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using DOANMOI.Models;
using PagedList;
namespace DOANMOI.Areas.Admin.Controllers
{
    public class Bai_vietController : Controller
    {
        private TapChi1Entities db = new TapChi1Entities();

        // Phương thức kiểm tra quyền truy cập
        private bool IsAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // GET: Admin/Bai_viet
        public ActionResult Index(String SearchString, int? page)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            var bai_viet = db.Bai_viet.Include(b => b.Danh_Muc).Include(b => b.User);
            // Lưu giá trị SearchString vào ViewBag
            ViewBag.CurrentSearchString = SearchString;
            if (!String.IsNullOrEmpty(SearchString))
            {
                if (!String.IsNullOrEmpty(SearchString))
                {
                    bai_viet = bai_viet.Where(s => s.TieuDe.Contains(SearchString) || s.NoiDung.Contains(SearchString) || s.Danh_Muc.TenDanhMuc.Contains(SearchString));
                }
            }
            // Kiểm tra nếu không có bài viết nào được tìm thấy
            // Nếu SearchString không rỗng và không có kết quả trả về ViewBag
            if (!String.IsNullOrEmpty(SearchString) && !bai_viet.Any())
            {
                ViewBag.Message = "Không tìm thấy ";
            }
            // khi sreach phải còn giữ những gì đã tìm kiếm 
            // Khai báo mỗi trang 4 sản phẩm 
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            // Thêm phương thức OrderBy để sắp xếp dữ liệu trước khi phân trang
            bai_viet = bai_viet.OrderBy(b => b.Thoi_Gian_Tao); // Sắp xếp theo thời gian tạo

            // Trả về các product được phân trang theo kích thước và số trang. 
            return View(bai_viet.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/Bai_viet/Details/5
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

        // GET: Admin/Bai_viet/Create
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

        // POST: Admin/Bai_viet/Create
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
            // Gán giá trị cho Thoi_Gian_Tao
            bai_viet.Thoi_Gian_Tao = DateTime.Now;
            //// Lấy UserID từ session
            //if (Session["UserID"] != null)
            //{
            //    bai_viet.UserID = Convert.ToInt32(Session["UserID"]);
            //}
            //else
            //{
            //    ModelState.AddModelError("UserID", "UserID không hợp lệ.");
            //}
            if (ModelState.IsValid)
            {
               
                db.Bai_viet.Add(bai_viet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Ghi lại lỗi nếu ModelState không hợp lệ
            foreach (var modelState in ModelState)
            {
                if (modelState.Value.Errors.Count > 0)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Key: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }
            }


            ViewBag.DanhMucID = new SelectList(db.Danh_Muc, "DanhMucID", "TenDanhMuc", bai_viet.DanhMucID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", bai_viet.UserID);
            return View(bai_viet);
        }

        // GET: Admin/Bai_viet/Edit/5
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

        // POST: Admin/Bai_viet/Edit/5
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
            // Tìm bài viết hiện tại từ cơ sở dữ liệu
            var existingBaiViet = db.Bai_viet.Find(bai_viet.BaiVietID);
            if (existingBaiViet == null)
            {
                return HttpNotFound();
            }

            // Nếu không có hình ảnh mới, giữ lại hình ảnh cũ
            if (string.IsNullOrEmpty(bai_viet.Hinh_Anh))
            {
                bai_viet.Hinh_Anh = existingBaiViet.Hinh_Anh;
            }
            // Giữ lại giá trị Thoi_Gian_Tao cũ
            bai_viet.Thoi_Gian_Tao = existingBaiViet.Thoi_Gian_Tao;



            if (ModelState.IsValid)
            {

                db.Entry(existingBaiViet).CurrentValues.SetValues(bai_viet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DanhMucID = new SelectList(db.Danh_Muc, "DanhMucID", "TenDanhMuc", bai_viet.DanhMucID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", bai_viet.UserID);
            return View(bai_viet);
        }

        // GET: Admin/Bai_viet/Delete/5
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

        // POST: Admin/Bai_viet/Delete/5
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
    }
}
