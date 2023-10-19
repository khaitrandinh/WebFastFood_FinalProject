using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCK_FinalProject.Models
{
    public class StatisticMemberview
    {
        public string FoodName { get; set; }
        public int TotalQuantity { get; set; }
        public string CustomerName { get; set; }

        public decimal Totalprice { get; set; }
    }
}