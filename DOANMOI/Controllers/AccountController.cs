using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DOANMOI.Models;


namespace DOANMOI.Controllers
{
    public class AccountController : Controller
    {
        private TapChi1Entities db = new TapChi1Entities();
        private bool IsAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }
        // GET: Account
        public ActionResult Index(string SearchString)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }

            ViewBag.CurrentString = SearchString;

            /* AsQueryable  cho phép bạn xây dựng truy vấn mà không thực hiện nó ngay lập tức,
               giúp giảm thiểu lượng dữ liệu được tải vào bộ nhớ và cải thiện hiệu suất. */
            var userslist = db.Users.AsQueryable();
            if (!String.IsNullOrEmpty(SearchString))
            {
                userslist = userslist.Where(s => s.UserName.Contains(SearchString) || s.RoleUser.Contains(SearchString));
            }
            // Nếu SearchString không rỗng và không có kết quả trả về ViewBag
            if (!String.IsNullOrEmpty(SearchString) && !userslist.Any())
            {
                ViewBag.Message = "Không tìm thấy ";
            }
            return View(userslist.ToList()); 
        }

        // GET: Account/Details/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,UserName,RoleUser,UserPassword,Email")] User user)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Account/Edit/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,RoleUser,UserPassword,Email")] User user)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Account/Delete/5
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
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("ThongBao", "TrangChu"); // Chuyển hướng nếu không phải Admin
            }
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
        [HttpGet]
        [AllowAnonymous]
    
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
           {
            // Bỏ qua kiểm tra ModelState cho UserName
            ModelState.Remove("UserName");
            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của đối tượng User
            {

                using (var db = new TapChi1Entities())
                             {
                                // Tìm người dùng theo email
                                var validUser = db.Users.FirstOrDefault(u => u.Email == user.Email );
                                if (validUser != null)
                                {
                                    // Kiểm tra mật khẩu
                                    if (validUser.UserPassword.Equals(user.UserPassword)) 
                                    {
                                        FormsAuthentication.SetAuthCookie(validUser.Email, false);
                                        Session["Role"] = validUser.RoleUser;  // Lưu vai trò người dùng trong session
                                        Session["UserName"] = validUser.UserName;
                                        Session["UserID"] = validUser.UserID;
                                        return RedirectToAction("home", "TrangChu"); // Chuyển hướng đến trang chính
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                                }
                             }
            }

            return View(user);
        }
        [HttpGet]
       
       
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                using (var db = new TapChi1Entities())
                {
                    // Kiểm tra xem tên đăng nhập đã tồn tại chưa
                    var existingUser = db.Users.FirstOrDefault(u => u.UserName == user.UserName);
                    if (existingUser == null)
                    {
                        user.RoleUser = "User"; // Gán vai trò là User
                        db.Users.Add(user);
                        db.SaveChanges();

                        // Sau khi đăng ký thành công, có thể chuyển hướng đến trang đăng nhập
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                    }
                }
            }
            return View(user);
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["Role"] = null;  // Xóa session vai trò người dùng
            Session["UserName"] = null; // Xóa session tên người dùng
            Session["UserID"] = null;
            return RedirectToAction("Login", "Account");
        }
    }
}

