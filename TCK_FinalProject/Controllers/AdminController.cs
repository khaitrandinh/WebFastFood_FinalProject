using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCK_FinalProject.Models;

namespace TCK_FinalProject.Controllers
{
    public class AdminController : Controller
    {
        private dbfinalProject_ASPDataContext db = new dbfinalProject_ASPDataContext();


        [HttpGet]
        //[Authorize(Roles = "admin")]  // Authorization for accessing the AdminDashboard action
        public ActionResult AdminDashboard(int? size, int? page, string searchString)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }


            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "12", Value = "12" });
            items.Add(new SelectListItem { Text = "24", Value = "24" });
            items.Add(new SelectListItem { Text = "48", Value = "48" });
            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.keyword = searchString;

            ViewBag.size = items; // viewbag dropdown list
            ViewBag.currentSize = size;
            if (page == null)
                page = 1;
            var all_book = (from food in db.foods select food).OrderBy(m => m.food_id);
            if (!string.IsNullOrEmpty(searchString))
            {
                all_book = (IOrderedQueryable<food>)all_book.Where(a => a.food_name.Contains(searchString));
            }
            int pageSize = (size ?? 3);
            int pageNum = page ?? 1;

            return View(all_book.ToPagedList(pageNum, pageSize));
        }

        
        public ActionResult StatisticMember(string searchString)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            ViewBag.keyword_cus = searchString;

            var statisticData = db.orderdetails
                    .GroupBy(od => new { od.food.food_name, od.order.customer.customer_name, od.price })
                    .Select(g => new StatisticMemberview
                    {
                        CustomerName = g.Key.customer_name,
                        FoodName = g.Key.food_name,
                        TotalQuantity = (int)g.Sum(od => od.quantity),
                        Totalprice = (decimal)g.Sum(od => od.price)
                    })
                 .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                statisticData = statisticData.Where(a => a.CustomerName.Contains(searchString)).ToList();
            }

            return View(statisticData);
        }


        [HttpGet]
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(FormCollection collection)
        {
            var username = collection["admin_username"];
            var password = collection["admin_password"];
            admin a = db.admins.SingleOrDefault(n => n.admin_username == username && n.admin_password == password);
            if (a != null)
            {
                ViewBag.ThongBao = "Congratulations on successful login";
                Session["Admin"] = a;
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                ViewBag.ThongBao = "Username or password is incorrect";
                return View("AdminLogin");
            }
        }



        [HttpGet]
        public ActionResult AdminLogout()
        {
            // Clear the admin session
            Session["Admin"] = null;

            // Redirect to the admin login page or any other page as needed
            return RedirectToAction("AdminLogin");
        }
        // GET: Admin


        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            var D_Book = db.foods.First(m => m.food_id == id);
            return View(D_Book);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            var D_Book = db.foods.Where(m => m.food_id == id).First();
            db.foods.DeleteOnSubmit(D_Book);
            db.SubmitChanges();
            return RedirectToAction("AdminDashboard");
        }

        public ActionResult Create()
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            // var newBook = new Book(); // Create a new Book object to use for adding
            // return View("Add", newBook);
            return View();
        }
        //[HttpPost]
        // [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(FormCollection collection, food s)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            var E_Name = collection["food_name"];
            var E_Image = collection["image"];
            var E_Price = Convert.ToDecimal(collection["price"]);
            var E_Updatedate = Convert.ToDateTime(collection["update_date"]);
            var E_Quantity = Convert.ToInt32(collection["Quantity_instock"]);
            if (string.IsNullOrEmpty(E_Name))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                s.food_name = E_Name.ToString();
                s.image = E_Image.ToString();
                s.price = E_Price;
                s.update_date = E_Updatedate;
                s.quantity_instock = E_Quantity;

                db.foods.InsertOnSubmit(s);
                db.SubmitChanges();

                return RedirectToAction("AdminDashboard");
            }
            return this.Create();
        }
        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            var E_Book = db.foods.FirstOrDefault(m => m.food_id == id);

            if (E_Book == null)
            {
                return HttpNotFound(); // Return a 404 Not Found if the book with the specified ID doesn't exist.

            }
            return View(E_Book);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection collection)
        {
            if (Session["Admin"] == null || Session["Admin"].ToString() == "")
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            var E_Book = db.foods.First(m => m.food_id == id);
            var E_Name = collection["food_name"];
            var E_Image = collection["image"];
            var E_Price = Convert.ToDecimal(collection["price"]);
            var E_Updatedate = Convert.ToDateTime(collection["update_date"]);
            var E_Quantity = Convert.ToInt32(collection["Quantity_instock"]);
            E_Book.food_id = id;
            if (string.IsNullOrEmpty(E_Name))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_Book.food_name = E_Name;
                E_Book.image = E_Image;
                E_Book.price = E_Price;
                E_Book.update_date = E_Updatedate;
                E_Book.quantity_instock = E_Quantity;

                UpdateModel(E_Book);

                db.SubmitChanges();

                return RedirectToAction("AdminDashboard");
            }
            return this.Edit(id);
        }

        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
    }
}