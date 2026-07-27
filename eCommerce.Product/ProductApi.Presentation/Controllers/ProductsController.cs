using e_commerce.sharedlibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();
            if(!products.Any())
            {
                return NotFound("No Products detected in the Database");
            }

            var(_, list) = ProductConversion.FromEntity(null!, products);
            return list.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if(product == null)
            {
                return NotFound("Product requested not found");
            }

            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is not null? Ok(_product) : NotFound(product.Name);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getEntity = ProductConversion.ToEntity(product);
            var response = await _productRepository.CreateAsync(getEntity);
            return response.Flag is true? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getEntity = ProductConversion.ToEntity(product);
            var response = await _productRepository.UpdateAsync(getEntity);
            return response.Flag is true? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            var productEntity = await _productRepository.FindByIdAsync(id);
            if (productEntity == null)
            {
                return NotFound(new Response(false, "Product requested to delete was not found"));
            }

            var response = await _productRepository.DeleteAsync(productEntity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
