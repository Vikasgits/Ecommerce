using E_commerce.DTOS;
using E_commerce.Models;
using System;

namespace E_commerce.Services
{
    public interface IProductService
    {
        public  Task BulkAddProductsAsync(List<Product> products);

        public List<PostProductDto> ParseExcelFile(Stream stream);

        public List<Product> GetAll();
        public Product GetById(Guid id);
        public bool Add(Product product);
        public Product Update(Product product);
        public Product GetByName(string name);
        public List<Product> GetByCategory(string categoryName , string subCategory, int? price);

        public Product GetProduct(Guid id);

        public List<Product> SearchProducts(string query);
        public bool Delete(Guid id);
        public bool AddStock(Guid id, int quantity);
        public bool RemoveStock(Guid id, int quantity);


    }
}
