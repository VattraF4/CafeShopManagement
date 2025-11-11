using System.Collections.Generic;
using OOADCafeShopManagement.Interface;
using OOADCafeShopManagement.Models;
using OOADCafeShopManagement.Repositories;

namespace OOADCafeShopManagement.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public List<Products> GetAllProducts() => _repo.GetAllProducts();
        public Products GetProductById(int id) => _repo.GetProductById(id);
        public List<Products> SearchProducts(string keyword) => _repo.SearchProducts(keyword);
        public bool AddProduct(Products product) => _repo.AddProduct(product);
        public bool UpdateProduct(Products product) => _repo.UpdateProduct(product);
        public bool DeleteProduct(int id) => _repo.DeleteProduct(id);
    }
}
