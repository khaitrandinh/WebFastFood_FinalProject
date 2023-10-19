using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCK_FinalProject.Models;

namespace TCK_FinalProject.Controllers
{
    public class CartController : Controller
    {
        dbfinalProject_ASPDataContext db = new dbfinalProject_ASPDataContext();
        public List<Cart> GetCarts()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        public ActionResult AddCart(int id, string strURL)
        {
            List<Cart> lstCart = GetCarts();
            Cart product = lstCart.Find(n => n.food_id == id);

            if (product == null)
            {
                // Create a new cart item if it doesn't exist in the cart
                product = new Cart(id);
                lstCart.Add(product);
                return Redirect(strURL);
            }
            else
            {
                // Increment the quantity if the item already exists in the cart
                product.iquantity++;
                return Redirect(strURL);
            }
        }

        // Calculate the total quantity of all items in the cart
        private int SumQuantity()
        {
            int tsl = 0;

            List<Cart> lstCart = Session["Cart"] as List<Cart>;

            if (lstCart != null)
            {
                tsl = lstCart.Sum(n => n.iquantity);
            }

            return tsl;
        }

        // Calculate the total number of products in the cart
        private int SumProductQuantity()
        {
            int tsl = 0;

            List<Cart> lstCart = Session["Cart"] as List<Cart>;

            if (lstCart != null)
            {
                tsl = lstCart.Count;
            }

            return tsl;
        }
        private double Total()
        {
            double tt = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                tt = lstCart.Sum(n => n.Total);
            }
            return tt;
        }

        public ActionResult Cart()
        {
            List<Cart> lstCart = GetCarts();
            ViewBag.SumQuantity = SumQuantity();
            ViewBag.Total = Total();
            ViewBag.SumProductQuantity = SumProductQuantity();
            return View(lstCart);
        }

        public ActionResult CartPartial()
        {
            ViewBag.SumQuantity = SumQuantity();
            ViewBag.Total = Total();
            ViewBag.SumProductQuantity = SumProductQuantity();
            return PartialView();
        }

        public ActionResult CartDelete(int id)
        {
            List<Cart> lstCart = GetCarts();
            Cart product = lstCart.SingleOrDefault(n => n.food_id == id);
            if (product != null)
            {
                lstCart.RemoveAll(n => n.food_id == id);
            }
            return RedirectToAction("Cart");
        }

        public ActionResult CartUpdate(int id, FormCollection collection)
        {
            List<Cart> lstCart = GetCarts();
            Cart product = lstCart.SingleOrDefault(n => n.food_id == id);

            if (product != null)
            {
                product.iquantity = int.Parse(collection["txtSoLg"].ToString());
            }

            return RedirectToAction("Cart");
        }

        public ActionResult AllCartDelete()
        {
            List<Cart> lstCart = GetCarts();
            lstCart.Clear();
            return RedirectToAction("Cart");
        }



        [HttpGet]
        public ActionResult PlaceOrder()
        {
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("Login", "User");
            }

            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Food");
            }

            List<Cart> lstCart = GetCarts();
            ViewBag.SumQuantity = SumQuantity();
            ViewBag.Total = Total();
            ViewBag.SumProductQuantity = SumProductQuantity();

            return View(lstCart);
        }

        [HttpPost]
        public ActionResult PlaceOrder(FormCollection collection)
        {
            order dh = new order();
            customer kh = (customer)Session["User"];
            food s = new food();

            List<Cart> gh = GetCarts();
            var delivery_date = String.Format("{0:MM/dd/yyyy}", collection["delivery_date"]);

            dh.customer_id = kh.customer_id;
            dh.order_date = DateTime.Now;
            dh.delivery_date = DateTime.Parse(delivery_date);
            dh.isship = false;
            dh.ispayment = false;

            db.orders.InsertOnSubmit(dh);
            db.SubmitChanges();
            /*add thêm*/
            var strSanPham = "";
            var thanhTien = decimal.Zero;
            var tongTien = decimal.Zero;
            foreach (Cart item in gh)
            {
                strSanPham += "<tr>";
                strSanPham += "<td>" + item.food_name + "<td>";
                strSanPham += item.iquantity + "<td>";
                strSanPham += string.Format("{0:N0}", item.price) + "VND";
                thanhTien += (decimal)item.price * item.iquantity;
                strSanPham += "<td>" + string.Format("{0:N0}", thanhTien) + "VND";
            }

            foreach (var item in gh)
            {
                orderdetail ctdh = new orderdetail();

                ctdh.order_id = dh.order_id;
                ctdh.food_id = item.food_id;
                ctdh.quantity = item.iquantity;
                ctdh.price = (decimal)item.price;

                s = db.foods.Single(n => n.food_id == item.food_id);
                s.quantity_instock -= ctdh.quantity;
                db.SubmitChanges();

                db.orderdetails.InsertOnSubmit(ctdh);

            }
            db.SubmitChanges();

            //send email
            tongTien = thanhTien;
            string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
            contentCustomer = contentCustomer.Replace("{{MaDon}}", dh.order_id.ToString());
            contentCustomer = contentCustomer.Replace("{{NgayDatHang}}", dh.delivery_date.ToString());
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", kh.customer_name);
            contentCustomer = contentCustomer.Replace("{{Phone}}", kh.numberphone);
            contentCustomer = contentCustomer.Replace("{{Email}}", kh.email);
            contentCustomer = contentCustomer.Replace("{{DiaChi}}", kh.address);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhTien.ToString());
            contentCustomer = contentCustomer.Replace("{{TongTien}}", string.Format("{0:N0}", tongTien));
            TCK_FinalProject.Models.Common.SendMail("ShopOnline", "Đơn hàng #" + dh.order_id.ToString(), contentCustomer, kh.email);

            Session["Cart"] = null;

            return RedirectToAction("ConfirmOrder", "Cart");
        }
        public ActionResult ConfirmOrder()
        {
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                return View();
            }
        }


        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
    }
}