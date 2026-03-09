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

        // Order Type Strategy Pattern fields
        public decimal ServiceCharge { get; set; } = 0;
        public decimal Tax { get; set; } = 0;
        public string OrderType { get; set; } = "Dine-In";
    }
}

