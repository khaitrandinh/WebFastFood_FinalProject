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
        public ActionResult Orders(string idorder)
        {
            if (Session["User"] != null)
            {
                // Lấy tên khách hàng từ đăng nhập
                customer customerName = Session["User"] as customer; // Tên khách hàng được lưu trong biến session hoặc nơi khác
                ViewBag.idorder = idorder;

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
                        total = g.Sum(x => x.price * x.quantity) ?? 0
                    }
                ).ToList();
                if (!string.IsNullOrEmpty(idorder))
                {
                    lst = lst.Where(a => a.orderID.ToString().Contains(idorder)).ToList();
                }
                return View(lst);
            }
            else
            {
                // Trong trường hợp không có khách hàng nào đăng nhập, bạn có thể xử lý điều này theo cách phù hợp với ứng dụng của bạn.
                return RedirectToAction("Login", "User");
            }
            
        }
        public ActionResult OrdersAll(string idorder)
        {
            if (Session["Admin"] != null)
            {

                ViewBag.idorder = idorder;
                List<Order> lst = (
                    from a in db.orders
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
                        total = g.Sum(x => x.price * x.quantity) ?? 0
                    }
                ).ToList();
                if (!string.IsNullOrEmpty(idorder))
                {
                    lst = lst.Where(a => a.orderID.ToString().Contains(idorder)).ToList();
                }
                return View(lst);
            }
            else
            {
                // Trong trường hợp không có khách hàng nào đăng nhập, bạn có thể xử lý điều này theo cách phù hợp với ứng dụng của bạn.
                return RedirectToAction("Index", "Food");
            }
        }

        public ActionResult OrderDetail(int id)
        {
            if (Session["Admin"] != null)
            {
                var orderDetails = (
                    from a in db.orderdetails
                    where a.order_id == id
                    select new OrderDetail
                    {
                        OrderID = a.order_id,
                        FoodName = a.food.food_name,
                        Quantity = (int)a.quantity,
                        Price = a.price
                    }
                ).ToList();

                var orderItem = (
                    from a in db.orders
                    where a.order_id == id
                    select new OrderItem
                    {
                        OrderID = a.order_id,
                        CustomerName = a.customer.customer_name,
                        OrderDate = a.order_date.Value,
                        DeliveryDate = a.delivery_date.Value,
                        IsShip = a.isship.Value,
                        IsPayment = a.ispayment.Value,
                        Total = orderDetails.Sum(x => x.TotalAmount),
                        OrderDetails = orderDetails
                    }
                ).FirstOrDefault();

                return View(orderItem);
            }
            else
            {
                // Trong trường hợp không có khách hàng nào đăng nhập, bạn có thể xử lý điều này theo cách phù hợp với ứng dụng của bạn.
                return RedirectToAction("Index", "Food");
            }
        }
        public ActionResult ODetail(int id)
        {
            var loggedInUser = Session["User"] as customer;

            if (loggedInUser != null)
            {
                var orderDetails = (
                    from a in db.orderdetails
                    where a.order_id == id && a.order.customer.customer_name == loggedInUser.customer_name
                    select new OrderDetail
                    {
                        OrderID = a.order_id,
                        FoodName = a.food.food_name,
                        Quantity = (int)a.quantity,
                        Price = a.price
                    }
                ).ToList();

                var orderItem = (
                    from a in db.orders
                    where a.order_id == id && a.customer.customer_name == loggedInUser.customer_name
                    select new OrderItem
                    {
                        OrderID = a.order_id,
                        CustomerName = a.customer.customer_name,
                        OrderDate = a.order_date.Value,
                        DeliveryDate = a.delivery_date.Value,
                        IsShip = a.isship.Value,
                        IsPayment = a.ispayment.Value,
                        Total = orderDetails.Sum(x => x.TotalAmount),
                        OrderDetails = orderDetails
                    }
                ).FirstOrDefault();

                if (orderItem != null)
                {
                    return View(orderItem);
                }
            }

            // Trong trường hợp không có khách hàng nào đăng nhập hoặc không tìm thấy đơn hàng, 
            // bạn có thể xử lý điều này theo cách phù hợp với ứng dụng của bạn.
            return RedirectToAction("Index", "Food");
        }

    }
}