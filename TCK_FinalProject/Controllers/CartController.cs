using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TCK_FinalProject.Models;
using log4net;

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
                strSanPham += item.price + ".000đ";
                thanhTien += (decimal)item.price * item.iquantity;
                strSanPham += "<td>" + thanhTien.ToString() + ".000đ";
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
            contentCustomer = contentCustomer.Replace("{{TongTien}}", tongTien.ToString());
            TCK_FinalProject.Models.Common.SendMail("ShopOnline", "Đơn hàng #" + dh.order_id.ToString(), contentCustomer, kh.email);

            Session["Cart"] = null;
            if(dh.ispayment == false)
            {
                return RedirectToAction("Checkout", "Cart", new { orderID = dh.order_id });
            }
            else
            {
                return RedirectToAction("ConfirmOrder", "Cart");
            }
        }
        public ActionResult Checkout(int orderID, FormCollection collection, Checkout c)
        {
            var code = new { Success = false, Code = -1, Url = "" };
            var CustomerName = collection["CustomerName"];
            var phone = collection["phone"];
            var Address = collection["Address"];
            var Email = collection["Email"];
            var TypePayment = Convert.ToInt32(collection["TypePaymentVM"]);
            var TypePaymentVN = Convert.ToBoolean(collection["TypePaymentVN"]);
            int id = orderID;
            var dh = db.orders.Where(m => m.order_id == id).First();
            if(dh == null)
            {
                return ViewBag["Error"] = "Your order do not contain in ";
            }
            else
            {
                c.CustomerName = CustomerName;
                c.Phone = phone;
                c.Address = Address;
                c.Email = Email;
                c.TypePayment = TypePayment;
                c.TypePaymentVN = TypePaymentVN;
                code = new { Success = true, Code = c.TypePayment, Url = "" };
                var url = UrlPayment(c.TypePayment ,c.TypePaymentVN, id);
                code = new { Success = true, Code = c.TypePayment, Url = url };
                return RedirectToAction("PaymentResult", "Cart");
            }
        }
        // Build 1 UrlPayment, sau đó gửi cho VNPay
        public string UrlPayment(int TypePayment, bool TypePaymentVN, int orderID)
        {
            // Mở database bảng order lấy order id cần thanh toán
            var order = db.orders.FirstOrDefault(x => x.order_id == orderID);
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhận kết quả trả về 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toán của VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Mã định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            var price = Total() * 100;
            vnpay.AddRequestData("vnp_Amount", (price * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePayment == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePayment == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePayment == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.order_date.ToString());
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());

            if (TypePaymentVN == true)
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }
            else if (TypePaymentVN == false)
            {
                vnpay.AddRequestData("vnp_Locale", "en");
            }
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.order_id);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.order_id.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            // Tạo 1 Url để chuyển client sang API của VNPay để thực hiện thanh toán
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            // Chuyển đến trang thanh toán VNPay
            return paymentUrl;
        }
        public ActionResult ConfirmOrder(){
            return View();
        }
        //Xử lý kết quả thanh toán từ VNPay
        public ActionResult PaymentResult(){
            // Lấy kết quả trả về
            string status = Request.QueryString["vnp_ResponseCode"];
            // Kiểm tra kết quả và thực hiện các xử lý tương ứng
            if (status == "00"){
                return RedirectToAction("ConfirmOrder");
            }
            else
            {
                return RedirectToAction("Error", "Cart");
            }
        }
        public ActionResult Error()
        {
            return View();
        }
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
    }
}