using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCK_FinalProject.Models
{
    public class OrderItem
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public bool IsShip { get; set; }
        public bool IsPayment { get; set; }
        public decimal Total { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}