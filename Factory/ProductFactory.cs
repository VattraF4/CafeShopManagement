using System;
using OOADCafeShopManagement.Models;

namespace OOADCafeShopManagement.Factory
{
    /// <summary>
    /// Factory Pattern for creating Product objects
    /// Handles different product types and proper initialization
    /// </summary>
    public interface IProductFactory
    {
        Products CreateProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0);
        Products CreateActiveProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0);
        Products CreateInactiveProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0);
        Products CreateBeverageProduct(string name, int supplierId, decimal price, decimal discount = 0);
        Products CreateFoodProduct(string name, int supplierId, decimal price, decimal discount = 0);
        Products CreateDessertProduct(string name, int supplierId, decimal price, decimal discount = 0);
    }

    public class ProductFactory : IProductFactory
    {
        // Default category IDs (these should match your database)
        private const int BEVERAGE_CATEGORY_ID = 1;
        private const int FOOD_CATEGORY_ID = 2;
        private const int DESSERT_CATEGORY_ID = 3;

        /// <summary>
        /// Creates a product with all parameters
        /// </summary>
        public Products CreateProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0)
        {
            ValidateProductParameters(name, categoryId, supplierId, price, discount);

            var product = new Products
            {
                Name = name,
                CategoryID = categoryId,
                SupplierID = supplierId,
                Price = price,
                Discount = discount,
                Status = "active"
            };

            product.Validate(); // Use the validation from the model
            return product;
        }

        /// <summary>
        /// Creates an active product
        /// </summary>
        public Products CreateActiveProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0)
        {
            var product = CreateProduct(name, categoryId, supplierId, price, discount);
            product.Activate();
            return product;
        }

        /// <summary>
        /// Creates an inactive product (for inventory management)
        /// </summary>
        public Products CreateInactiveProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0)
        {
            var product = CreateProduct(name, categoryId, supplierId, price, discount);
            product.Deactivate();
            return product;
        }

        /// <summary>
        /// Creates a beverage product (Coffee, Tea, Juice, etc.)
        /// </summary>
        public Products CreateBeverageProduct(string name, int supplierId, decimal price, decimal discount = 0)
        {
            return CreateActiveProduct(name, BEVERAGE_CATEGORY_ID, supplierId, price, discount);
        }

        /// <summary>
        /// Creates a food product (Sandwiches, Pastries, etc.)
        /// </summary>
        public Products CreateFoodProduct(string name, int supplierId, decimal price, decimal discount = 0)
        {
            return CreateActiveProduct(name, FOOD_CATEGORY_ID, supplierId, price, discount);
        }

        /// <summary>
        /// Creates a dessert product (Cakes, Cookies, etc.)
        /// </summary>
        public Products CreateDessertProduct(string name, int supplierId, decimal price, decimal discount = 0)
        {
            return CreateActiveProduct(name, DESSERT_CATEGORY_ID, supplierId, price, discount);
        }

        /// <summary>
        /// Creates a product with a percentage discount
        /// </summary>
        public Products CreateProductWithPercentDiscount(string name, int categoryId, int supplierId, 
            decimal price, decimal discountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100.", nameof(discountPercent));

            decimal discountAmount = price * (discountPercent / 100);
            return CreateProduct(name, categoryId, supplierId, price, discountAmount);
        }

        /// <summary>
        /// Creates a promotional product (with higher discount)
        /// </summary>
        public Products CreatePromotionalProduct(string name, int categoryId, int supplierId, 
            decimal price, decimal discountPercent)
        {
            if (discountPercent < 10)
                throw new ArgumentException("Promotional discount must be at least 10%.", nameof(discountPercent));

            return CreateProductWithPercentDiscount(name, categoryId, supplierId, price, discountPercent);
        }

        /// <summary>
        /// Creates a product from database data
        /// </summary>
        public Products CreateFromData(int id, string name, int categoryId, int supplierId, 
            decimal price, decimal discount, string status, string categoryName = null, string supplierName = null)
        {
            return new Products
            {
                ID = id,
                Name = name,
                CategoryID = categoryId,
                SupplierID = supplierId,
                Price = price,
                Discount = discount,
                Status = status,
                CategoryName = categoryName,
                SupplierName = supplierName
            };
        }

        /// <summary>
        /// Validates product creation parameters
        /// </summary>
        private void ValidateProductParameters(string name, int categoryId, int supplierId, decimal price, decimal discount)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            if (categoryId <= 0)
                throw new ArgumentException("Valid category is required.", nameof(categoryId));

            if (supplierId <= 0)
                throw new ArgumentException("Valid supplier is required.", nameof(supplierId));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative.", nameof(price));

            if (discount < 0)
                throw new ArgumentException("Discount cannot be negative.", nameof(discount));

            if (discount > price)
                throw new ArgumentException("Discount cannot exceed price.", nameof(discount));
        }
    }
}
