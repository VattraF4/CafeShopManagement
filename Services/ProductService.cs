using System;
using System.Collections.Generic;
using System.Linq;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;
using OOADCafeShopManagement.Repositories;
using OOADCafeShopManagement.Factory;

namespace OOADCafeShopManagement.Services
{
    /// <summary>
    /// Product Service Layer - Business logic for product management
    /// Uses Repository Pattern for data access
    /// Uses Factory Pattern for object creation
    /// </summary>
    public class ProductService
    {
        private readonly IProductRepository _repo;
        private readonly IProductFactory _productFactory;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
            _productFactory = new ProductFactory();
        }

        public ProductService(IProductRepository repo, IProductFactory productFactory)
        {
            _repo = repo;
            _productFactory = productFactory;
        }

        public List<Products> GetAllProducts() => _repo.GetAllProducts();

        public List<Products> GetActiveProducts() => _repo.GetActiveProducts();

        public Products GetProductById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product ID must be greater than zero.", nameof(id));

            return _repo.GetProductById(id);
        }

        public List<Products> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return GetActiveProducts();

            return _repo.SearchProducts(keyword);
        }

        public List<Products> GetProductsByCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than zero.", nameof(categoryId));

            return _repo.GetProductsByCategory(categoryId);
        }

        public List<Products> GetProductsBySupplier(int supplierId)
        {
            if (supplierId <= 0)
                throw new ArgumentException("Supplier ID must be greater than zero.", nameof(supplierId));

            return _repo.GetProductsBySupplier(supplierId);
        }

        public bool AddProduct(string name, int categoryId, int supplierId, decimal price, decimal discount = 0)
        {
            // Validation is handled by ProductFactory

            // Check for duplicate product name
            if (_repo.ProductExists(name))
            {
                throw new Exception($"Product '{name}' already exists.");
            }

            // Use ProductFactory to create product object
            var product = _productFactory.CreateProduct(name, categoryId, supplierId, price, discount);

            return _repo.AddProduct(product);
        }

        public bool UpdateProduct(int id, string name, int categoryId, int supplierId, decimal price, decimal discount, string status)
        {
            // Validation
            if (id <= 0)
                throw new ArgumentException("Invalid product ID.", nameof(id));

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

            // Check if product exists
            var existingProduct = _repo.GetProductById(id);
            if (existingProduct == null)
            {
                throw new Exception("Product not found.");
            }

            // Check for duplicate name (excluding current product)
            if (_repo.ProductExists(name, id))
            {
                throw new Exception($"Another product with name '{name}' already exists.");
            }

            // Update product object
            existingProduct.Name = name;
            existingProduct.CategoryID = categoryId;
            existingProduct.SupplierID = supplierId;
            existingProduct.Price = price;
            existingProduct.Discount = discount;
            existingProduct.Status = status ?? "active";

            // Validate product
            existingProduct.Validate();

            return _repo.UpdateProduct(existingProduct);
        }

        public bool DeleteProduct(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid product ID.", nameof(id));

            var product = _repo.GetProductById(id);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            return _repo.DeleteProduct(id);
        }

        public bool ActivateProduct(int id)
        {
            var product = _repo.GetProductById(id);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            product.Activate();
            return _repo.UpdateProduct(product);
        }

        public bool DeactivateProduct(int id)
        {
            var product = _repo.GetProductById(id);
            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            product.Deactivate();
            return _repo.UpdateProduct(product);
        }

        public List<Products> GetProductsWithDiscount()
        {
            return GetAllProducts().Where(p => p.HasDiscount()).ToList();
        }

        public List<Products> GetInactiveProducts()
        {
            return _repo.GetProductsByStatus("inactive");
        }

        public decimal GetAveragePrice()
        {
            var products = GetActiveProducts();
            if (products.Count == 0) return 0;
            return products.Average(p => p.Price);
        }

        public Products GetMostExpensiveProduct()
        {
            var products = GetActiveProducts();
            return products.OrderByDescending(p => p.Price).FirstOrDefault();
        }

        public Products GetCheapestProduct()
        {
            var products = GetActiveProducts();
            return products.OrderBy(p => p.Price).FirstOrDefault();
        }
    }
}
