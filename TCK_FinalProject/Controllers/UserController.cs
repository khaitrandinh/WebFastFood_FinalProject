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

        /*(?=.*[A-Z]): At least one uppercase letter.
        (?=.*\d): At least one digit.
        (?=.*\W): At least one special character (non-alphanumeric).*/
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
                    // Validate password complexity
                    if (!IsPasswordValid(password))
                    {
                        ViewData["passwordComplexity"] = "Password must contain at least one uppercase letter, one digit, and one special character.";
                        return View();
                    }

                    // Check if username already exists
                    var existingUsername = db.customers.FirstOrDefault(x => x.username == username);
                    if (existingUsername != null)
                    {
                        ViewData["usernameExists"] = "Username already exists. Please choose a different username.";
                        return View();
                    }

                    // Check if email already exists
                    var existingEmail = db.customers.FirstOrDefault(x => x.email == email);
                    if (existingEmail != null)
                    {
                        ViewData["emailExists"] = "Email already exists. Please use a different email.";
                        return View();
                    }

                    // Check if phone number already exists
                    var existingPhoneNumber = db.customers.FirstOrDefault(x => x.numberphone == numberphone);
                    if (existingPhoneNumber != null)
                    {
                        ViewData["phoneNumberExists"] = "Phone number already exists. Please use a different phone number.";
                        return View();
                    }

                    // Continue with the rest of your registration logic...
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

            return View();
        }
        private bool IsPasswordValid(string password)
        {
            // Password must contain at least one uppercase letter, one digit, and one special character
            return System.Text.RegularExpressions.Regex.IsMatch(password, @"^(?=.*[A-Z])(?=.*\d)(?=.*\W).*$");
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

            customer c = db.customers.FirstOrDefault(n => n.username == username && n.password == password);
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