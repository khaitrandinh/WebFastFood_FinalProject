using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCK_FinalProject.Models
{
    public class OrderDetail
    {
        public int OrderID { get; set; }
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }

        public decimal TotalAmount
        {
            get { return (decimal)(Quantity * Price); }
        }
    }
}