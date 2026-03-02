using System;

namespace OOADCafeShopManagement.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string Note { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public int UserID { set; get; }
        public DateTime OrderDate { get; set; }
    }
}

