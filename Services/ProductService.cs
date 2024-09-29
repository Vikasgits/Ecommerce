using E_commerce.DTOS;
using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Repository;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IEntityRepository<Product> _productRepository;

        public ProductService(IEntityRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public bool Add(Product product)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                if (product == null) throw new InvalidProductException("Invalid Product Details");
                var existProduct = GetByName(product.Name);
                if (existProduct != null) throw new ProductNameAlreadyExistException("Product with that name already exist");
                //initiateTrans.Commit();
                var ans = _productRepository.Add(product);
                initiateTrans.Commit();
                return ans;
            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }
        }


        public bool AddStock(Guid id,int quantity)
        {
            
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                var getProduct = GetProdById(id);
                getProduct.InStock += quantity;
                Update(getProduct);
                initiateTrans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }

        }

        public bool Delete(Guid id)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                var getProduct = GetById(id);
                initiateTrans.Commit();
                return _productRepository.Delete(getProduct);

            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }
        }

        public bool RemoveStock(Guid id, int quantity)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                var getProduct = GetProdById(id);
                if (getProduct.InStock < quantity)
                {
                    throw new InsufficientStockException("Insufficient stock to decrement.");
                };
                getProduct.InStock -= quantity;
                Update(getProduct);
                initiateTrans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }
        }

        public Product GetProdById(Guid id)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                var product = _productRepository.Get().Where(prod => prod.ProductId == id).FirstOrDefault();
                if (product == null)
                {
                    throw new ProductNotFoundException("No such Stock found to be added/deleted");
                }
                initiateTrans.Commit();

                return product;
            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }
        }
        public Product GetProduct(Guid id)   ///con
        {
            var product = _productRepository.Get().Where(prod=> prod.ProductId == id).FirstOrDefault();
            return product;
        }

            public List<Product> GetAll()
        {
            var prodList = _productRepository.Get().Where(prod=>prod.IsAvailable==true).ToList();
            return prodList;
        }

        public Product GetById(Guid id)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                var product = _productRepository.Get(id);
                if (product == null)
                {
                    throw new ProductNotFoundException("No such Product found to be upadted/deleted");
                }
                initiateTrans.Commit();

                return product;


            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }
        }

        public Product GetByName(string name)
        {
            var product = _productRepository.Get().Where(prod => prod.Name == name).FirstOrDefault();
            return product;
        }

        public Product Update(Product product)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            try
            {
                Product updatedProduct = null;
                if (product == null) throw new InvalidProductException("Invalid Product Details");
                var existingProduct = _productRepository.Get().AsNoTracking()
                    .FirstOrDefault(prod => prod.Name == product.Name && prod.ProductId != product.ProductId);



                if (existingProduct != null)
                {
                    throw new ProductNameAlreadyExistException("Product with a similar name already exists");
                }
                var value = _productRepository.Update(product);
                initiateTrans.Commit();

                return value;
            }
            catch (Exception ex)
            {
                initiateTrans.Rollback();
                throw ex;
            }
        }

        public List<Product> SearchProducts(string query)
        {

                if (string.IsNullOrEmpty(query)) return new List<Product>();
                var lowerQuery = query.ToLower();

                var prodList = _productRepository.Get().Where(prod => prod.Name.ToLower().Contains(lowerQuery) ||
                                                           prod.Description.ToLower().Contains(lowerQuery)).ToList();
                return prodList;
        }


        public List<Product> GetByCategory(string categoryName, string subCategory, int? price)
        {
            var initiateTrans = _productRepository.BeginTransaction();
            var prodList= _productRepository.Get().Where(prod=>prod.Category.ToLower()==categoryName.ToLower()  && prod.SubCategory==subCategory).ToList();
            if (price.HasValue )
            {
                prodList = prodList.Where(prod => prod.Price <= price.Value).ToList();
            }

            return prodList;
        }

        public async Task BulkAddProductsAsync(List<Product> products)
        {
            var context = _productRepository.GetContext(); 
            context.Database.SetCommandTimeout(1500);
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    await context.BulkInsertAsync(products);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error during bulk insert: {ex.Message}", ex);
                }
            }
        }

        public List<PostProductDto> ParseExcelFile(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var productDtos = new List<PostProductDto>();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Assuming first row is headers
                {
                    var productDto = new PostProductDto
                    {
                        Name = worksheet.Cells[row, 1].Text,
                        Description = worksheet.Cells[row, 2].Text,
                        Price = double.Parse(worksheet.Cells[row, 5].Text),
                        Category = worksheet.Cells[row, 3].Text,
                        SubCategory = worksheet.Cells[row, 4].Text,
                        FilePath = worksheet.Cells[row, 6].Text // Assuming URL is in the 7th column
                    };

                    // Validate URL format if needed
                    if (!Uri.IsWellFormedUriString(productDto.FilePath, UriKind.Absolute))
                    {
                        throw new InvalidProductException($"Invalid Image URL in row {row}");
                    }

                    productDtos.Add(productDto);
                }
            }

            return productDtos;
        }

    }
}
