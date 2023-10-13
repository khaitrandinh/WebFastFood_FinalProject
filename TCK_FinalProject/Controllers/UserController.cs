using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCK_FinalProject.Models;

namespace TCK_FinalProject.Controllers
{
    public class UserController : Controller
    {
        dbfinalProject_ASPDataContext db = new dbfinalProject_ASPDataContext();
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection, customer c)
        {
            var name = collection["customer_name"];
            var username = collection["username"];
            var password = collection["password"];
            var confirmpassword = collection["confirmpassword"];
            var email = collection["email"];
            var address = collection["address"];
            var numberphone = collection["numberphone"];
            var dob = String.Format("{0:MM/dd/yyyy}", collection["dob"]);

            if (String.IsNullOrEmpty(confirmpassword))
            {
                ViewData["enterpassword"] = "Must enter password to confirm!";
            }
            else
            {
                if (!password.Equals(confirmpassword))
                {
                    ViewData["samepassword"] = "Password and confirmation password must be the same";
                }
                else
                {
                    c.customer_name = name;
                    c.username = username;
                    c.password = password;
                    c.email = email;
                    c.address = address;
                    c.numberphone = numberphone;
                    c.dob = DateTime.Parse(dob);
                    db.customers.InsertOnSubmit(c);
                    db.SubmitChanges();
                    return RedirectToAction("Register");
                }
            }
            return this.Register();
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["username"];
            var password = collection["password"];
            customer c = db.customers.SingleOrDefault(n => n.username == username && n.password == password);
            if (c != null)
            {
                ViewBag.ThongBao = "Congratulations on successful login";
                Session["User"] = c;
                return RedirectToAction("Index", "Food");
            }
            else
            {
                ViewBag.ThongBao = "Username or password is incorrect";
                return this.Login();
            }

        }

        [HttpGet]
        public ActionResult Logout()
        {
            // Clear the user session
            Session["User"] = null;

            // Redirect to the login page or any other page as needed
            return RedirectToAction("Login", "User");
        }
    }
}