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
        public ActionResult Index(int? size, int? page, string searchString)
        {
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
    }
}