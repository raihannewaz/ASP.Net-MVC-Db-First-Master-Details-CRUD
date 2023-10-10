using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Newaz_1271713.Models.ViewModels
{
    public class CustomerOrderVM
    {
        [Required(ErrorMessage = "Manually Input Customer Id and can't be duplicate")]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string ImagePath { get; set; }

        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int OrderID { get; set; }

        [Required(ErrorMessage ="Cant be blank")]
        public int OrderNo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "01-01-2023", "16-05-2023", ErrorMessage = "The date must be between {1} and {2}.")]
        public DateTime OrderDate { get; set; }


        public Nullable<bool> OrderStatus { get; set; }

        public List<Order> Orders { get; set; }
        public Customer Customers { get; set; }
    }
}