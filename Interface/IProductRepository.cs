using OOADCafeShopManagement.Models;
using System.Collections.Generic;

namespace OOADCafeShopManagement.Interface
{
    public interface IProductRepository
    {
        List<Products> GetAllProducts();
        List<Products> GetActiveProducts();
        Products GetProductById(int id);
        List<Products> SearchProducts(string keyword);
        List<Products> GetProductsByCategory(int categoryId);
        List<Products> GetProductsBySupplier(int supplierId);
        List<Products> GetProductsByStatus(string status);
        bool AddProduct(Products product);
        bool UpdateProduct(Products product);
        bool DeleteProduct(int id);
        bool ProductExists(string name, int excludeId = 0);
    }
}



