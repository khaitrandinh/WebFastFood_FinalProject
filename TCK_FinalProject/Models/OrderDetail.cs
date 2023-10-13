using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCK_FinalProject.Models
{
    public class OrderDetail
    {
        public int orderID { get; set; }
        public string foodName { get; set; }
        public int quantity { get; set; }
        public decimal? price { get; set; }


        public decimal totalAmount
        {
            get { return (decimal)(quantity * price); }
        }
    }
}