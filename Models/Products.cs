using System;

namespace OOADCafeShopManagement.Models
{
    /// <summary>
    /// Product domain model - Contains only properties and business logic
    /// Data access is handled by ProductRepository (Repository Pattern)
    /// </summary>
    public class Products
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public string Status { get; set; }

        public Products() { }

        public Products(string name, int categoryId, int supplierId, decimal price, decimal discount)
        {
            Name = name;
            CategoryID = categoryId;
            SupplierID = supplierId;
            Price = price;
            Discount = discount;
            Status = "active";
        }

        // Business logic methods
        public decimal GetPriceAfterDiscount()
        {
            return Price - Discount;
        }

        public decimal GetDiscountPercentage()
        {
            if (Price == 0) return 0;
            return (Discount / Price) * 100;
        }

        public bool IsActive()
        {
            return Status?.Equals("active", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public bool IsInactive()
        {
            return Status?.Equals("inactive", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public bool HasDiscount()
        {
            return Discount > 0;
        }

        public void Activate()
        {
            Status = "active";
        }

        public void Deactivate()
        {
            Status = "inactive";
        }

        public string GetDisplayInfo()
        {
            return $"{Name} - ${GetPriceAfterDiscount():F2} ({CategoryName})";
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Product name is required.");

            if (Price < 0)
                throw new ArgumentException("Price cannot be negative.");

            if (Discount < 0)
                throw new ArgumentException("Discount cannot be negative.");

            if (Discount > Price)
                throw new ArgumentException("Discount cannot exceed price.");

            if (CategoryID <= 0)
                throw new ArgumentException("Valid category is required.");

            if (SupplierID <= 0)
                throw new ArgumentException("Valid supplier is required.");
        }
    }
}
