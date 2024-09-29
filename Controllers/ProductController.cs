using AutoMapper;
using E_commerce.DTOS;
using E_commerce.Exceptions;
using E_commerce.Models;
using E_commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetAllProducts()
        {
            List<Product> productList = _productService.GetAll();
            return Ok(productList);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetProduct(Guid id)
        {
            try
            {

                var product = _productService.GetById(id);
                return Ok(product);

            }
            catch (ProductNotFoundException pnfe)
            {
                return NotFound(new { error = $"{pnfe.Message},{pnfe.StatusCode}" });
            }
            catch (InvalidProductException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
        }

        [HttpGet("{category}/{subCategory}")]
        public IActionResult GetProductsByCategory(string category, string subCategory, [FromQuery] int? price)
        {

            List<Product> prodList = _productService.GetByCategory(category, subCategory, price);
            return Ok(prodList);
        }

        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { error = "Search query cannot be empty." });
            }

            List<Product> prodList = _productService.SearchProducts(query);
            return Ok(prodList);
        }




        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult>  AddNewProduct([FromForm] PostProductDto postProductDto) {
            try {
                if ( postProductDto.ImageFile == null   )
                {
                    return BadRequest("Invalid File");
                }
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(postProductDto.ImageFile.FileName)}";
                var imagePath = Path.Combine("wwwroot", "images", fileName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await postProductDto.ImageFile.CopyToAsync(fileStream);
                }
                Product product = _mapper.Map<Product>(postProductDto);
                product.ProductId = Guid.NewGuid();
                product.FilePath = $"images/{fileName}";
                _productService.Add(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, new { productId=product.ProductId} );
                
            }
            catch (InvalidProductException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
            catch (ProductNameAlreadyExistException pnae)
            {
                return BadRequest(new { error = $"{pnae.Message}, {pnae.StatusCode}" });
            }
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public  async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto updateProductDto)
        {
            try
            {
                Product getProduct = _productService.GetById(updateProductDto.ProductId);
                if (getProduct.FilePath != updateProductDto.FilePath)
                {
                    if (updateProductDto.ImageFile == null || updateProductDto.ImageFile.Length == 0)
                    {
                        return BadRequest("Invalid Product Picture");
                    }
                    var filename = $"{Guid.NewGuid()}{Path.GetExtension(updateProductDto.ImageFile.FileName)}";
                    var imagePath = Path.Combine("wwwroot", "images", filename);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await updateProductDto.ImageFile.CopyToAsync(fileStream);
                    }
                    updateProductDto.FilePath = $"images/{filename}";
                }
                var product = _mapper.Map<Product>(updateProductDto);
                var updatedProduct = _productService.Update(product);
                return Ok(updatedProduct);
            }
            catch (InvalidProductException ipe)
            {
                return BadRequest(new { error = $"{ipe.Message},{ipe.StatusCode}" });
            }
            catch (ProductNameAlreadyExistException pnae)
            {
                return BadRequest(new { error = $"{pnae.Message}, {pnae.StatusCode}" });
            }

        }

        [HttpPost("add-stock/{id:guid}")]
        public IActionResult AddStock(Guid id, [FromQuery] int quantity)
        {
            try
            {
               _productService.AddStock(id, quantity);
                return Ok(new { message = "Stock added successfully"});
            }
            catch (ProductNotFoundException pnfe)
            {
                return NotFound(new { error = $"{pnfe.Message},{pnfe.StatusCode}" });
            }
        }

        [HttpPost("decrement-stock/{id:guid}")]
        public IActionResult DecrementStock(Guid id, [FromQuery] int quantity)
        {
            try
            {
                _productService.RemoveStock(id, quantity);
                return Ok(new { message = "Stock removed successfully" });
            }
            catch (ProductNotFoundException pnfe)
            {
                return NotFound(new { error = $"{pnfe.Message},{pnfe.StatusCode}" });
            }
        }




        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            try
            {
                _productService.Delete(id);
                return Ok(new { message = "Product Deleted" });
            }
            catch (ProductNotFoundException pnfe)
            {
                return NotFound(new { error = $"{pnfe.Message},{pnfe.StatusCode}" });
            }


        }

        [HttpPost("bulk-upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    var productDtos = _productService.ParseExcelFile(stream);

                   

                    var products = productDtos.Select(dto =>
                    {
                        var product = _mapper.Map<Product>(dto);
                        product.ProductId = Guid.NewGuid();
                        return product;
                    }).ToList();


                    await _productService.BulkAddProductsAsync(products);
                }

                return Ok(new { message = "Products Uploaded Successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

      



       

    }


}
