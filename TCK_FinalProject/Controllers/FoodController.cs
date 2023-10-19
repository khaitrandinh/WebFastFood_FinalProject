using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCK_FinalProject.Models;

namespace TCK_FinalProject.Controllers
{
    public class FoodController : Controller
    {
        // GET: Food
        dbfinalProject_ASPDataContext db = new dbfinalProject_ASPDataContext();
        public ActionResult Index(int? size, int? page, string searchString, string sort)
        {
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem { Text = "6", Value = "6" },
                new SelectListItem { Text = "12", Value = "12" },
                new SelectListItem { Text = "24", Value = "24" },
                new SelectListItem { Text = "48", Value = "48" }
            };

            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                    item.Selected = true;
            }

            ViewBag.keyword = searchString;
            ViewBag.size = items;
            ViewBag.currentSize = size;

            var all_book = db.foods.AsQueryable();

            switch (sort)
            {
                case "name_desc":
                    all_book = all_book.OrderByDescending(m => m.food_name);
                    break;
                case "price":
                    all_book = all_book.OrderBy(m => m.price);
                    break;
                case "price_desc":
                    all_book = all_book.OrderByDescending(m => m.price);
                    break;
                default:
                    all_book = all_book.OrderBy(m => m.food_name); // Default sort by name ascending
                    sort = "name_asc"; // Ensure default value is set
                    break;
            }

            int pageSize = (size ?? 6);
            int pageNum = page ?? 1;

            ViewBag.SortOrder = sort; // Pass the sort order to the view

            return View(all_book.ToPagedList(pageNum, pageSize));
        }


        public ActionResult Detail(int id)
        {
            var D_Food = db.foods.Where(m => m.food_id == id).First();
            return View(D_Food);
        }
        public ActionResult Statistic(string foodName)
        {
            // Get the current user from the session
            customer currentUser = Session["User"] as customer;

            ViewBag.food = foodName;

            // Check if the user is logged in
            if (currentUser != null)
            {
                // Filter the statistics based on the current user
                var statisticData = db.orderdetails
                    .Where(od => od.order.customer_id == currentUser.customer_id)
                    .GroupBy(od => od.food.food_name)
                    .Select(g => new StatisticViewModel
                    {
                        FoodName = g.Key,
                        TotalQuantity = (int)g.Sum(od => od.quantity)
                    })
                    .ToList();
                if (!string.IsNullOrEmpty(foodName))
                {
                    statisticData = statisticData.Where(a => a.FoodName.Contains(foodName)).ToList();
                }

                return View(statisticData);
            }
            // If the user is not logged in, you may want to redirect to the login page or handle it in some way
            return RedirectToAction("Login", "User");
        }
        [HttpGet]
        public ActionResult Comment(int id)
        {
            customer currentUser = Session["User"] as customer;
            if (currentUser != null)
            {
                ViewBag.Food = db.foods.First(a => a.food_id == id);
                return View("Comment");
            }
            else
            {
                // Trong trường hợp không có khách hàng nào đăng nhập, bạn có thể xử lý điều này theo cách phù hợp với ứng dụng của bạn.
                return RedirectToAction("Login", "User");
            }

        }
        [HttpPost]
        public ActionResult SendReview(review review, double rating)
        {
            customer username = Session["username"] as customer;
            review.review_date = DateTime.Now;
            review.customer_id = db.customers.First(a => a.username.Equals(username)).customer_id;
            review.rating = rating;
            db.reviews.InsertOnSubmit(review);
            db.SubmitChanges();
            return RedirectToAction("Comment", "Food",new {id = review.food_id });
        }
    }
}