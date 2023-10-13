using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TCK_FinalProject.Models
{
    public class Cart
    {
        dbfinalProject_ASPDataContext db = new dbfinalProject_ASPDataContext();
        public int food_id { get; set; }
        [Display(Name = "Food Name")]
        public string food_name { get; set; }
        [Display(Name = "Cover image")]
        public string image { get; set; }

        [Display(Name = "Price")]
        public Double price { get; set; }

        [Display(Name = "Quantity")]
        public int iquantity { get; set; }

        [Display(Name = "Total")]
        public Double Total
        {
            get { return iquantity * price; }
        }
        public Cart(int id)
        {
            food_id = id;
            food f = db.foods.Single(n => n.food_id == food_id);
            food_name = f.food_name;
            image = f.image;
            price = double.Parse(f.price.ToString());
            iquantity = 1;
        }
    }
}