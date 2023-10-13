using TCK_FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace TCK_FinalProject.Controllers
{
    public class OrderController : Controller
    {
        dbfinalProject_ASPDataContext db = new dbfinalProject_ASPDataContext();
        public ActionResult Orders()
        {
            if (Session["User"] != null)
            {
                // Lấy tên khách hàng từ đăng nhập
                customer customerName = Session["User"] as customer; // Tên khách hàng được lưu trong biến session hoặc nơi khác

                List<Order> lst = (
                    from a in db.orders
                    where a.customer.customer_name == customerName.customer_name // Chỉ lấy đơn hàng của khách hàng đăng nhập
                    join b in db.orderdetails on a.order_id equals b.order_id
                    group b by b.order_id into g
                    select new Order
                    {
                        orderID = g.First().order.order_id,
                        customerName = g.First().order.customer.customer_name,
                        isShip = g.First().order.isship.Value,
                        isPayment = g.First().order.ispayment.Value,
                        deliveryDate = g.First().order.delivery_date.Value,
                        orderDate = g.First().order.order_date.Value,
                        total = g.Sum(x => x.price) ?? 0
                    }
                ).ToList();

                return View(lst);
            }
            else
            {
                // Trong trường hợp không có khách hàng nào đăng nhập, bạn có thể xử lý điều này theo cách phù hợp với ứng dụng của bạn.
                return RedirectToAction("Login", "User");
            }
            
        }

        public class OrderDetailViewModel
        {
            public int OrderId { get; set; }
            public List<OrderDetail> OrderDetails { get; set; }
            public decimal TotalAmount { get; set; }
        }
        public ActionResult OrderDetail(int orderId)
        {
            if (Session["User"] != null)
            {
                customer currentUser = Session["User"] as customer;

                var orderDetails = (
                    from od in db.orderdetails
                    where od.order_id == orderId && od.order.customer.customer_id == currentUser.customer_id
                    join b in db.foods on od.food_id equals b.food_id
                    select new OrderDetail
                    {
                        orderID = od.order_id,
                        foodName = b.food_name,
                        quantity = (int)od.quantity,
                        price = od.price
                    }
                ).ToList();

                decimal totalAmount = orderDetails.Sum(item => item.totalAmount);

                var viewModel = new OrderDetailViewModel
                {
                    OrderId = orderId,
                    OrderDetails = orderDetails,
                    TotalAmount = totalAmount
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }








    }
}