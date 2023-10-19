using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TCK_FinalProject.Models
{
    public class Review
    {
        public int reviewID { get; set; }
        public int food_id { get; set; }
        public int customer_id { get; set; }
        public double rating { get; set; }
        public string comment { get; set; }
        public DateTime review_date { get; set; }
    }
}