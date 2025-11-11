using OOADCafeShopManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOADCafeShopManagement.Interface
{
    public interface IProductRepository
    {
        List<Products> GetAllProducts();
        Products GetProductById(int id);
        List<Products> SearchProducts(string keyword);
        bool AddProduct(Products product);
        bool UpdateProduct(Products product);
        bool DeleteProduct(int id);
    }
}



