using Microsoft.AspNetCore.Mvc;
using UnitTestExample.Web.Helpers;
using UnitTestExample.Web.Models;
using UnitTestExample.Web.Repositories;

namespace UnitTestExample.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        #region Ctor
        private readonly IRepository<Product> _productRepository;

        public ProductsApiController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        #endregion

        #region GetProducts
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetAllAsync();

            return Ok(products);
        }
        #endregion

        #region GetProduct
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        #endregion

        #region PutProduct
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _productRepository.Update(product);

            return NoContent();
        }
        #endregion

        #region PostProduct
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _productRepository.CreateAsync(product);

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }
        #endregion

        #region DeleteProduct
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _productRepository.Delete(product);
            return NoContent();
        }
        #endregion

        #region ProductExists
        private bool ProductExists(int id)
        {
            Product product = _productRepository.GetByIdAsync(id).Result;
            if (product == null)
                return false;
            else
                return true;
        }
        #endregion

        #region HelperAdd
        [HttpGet("{number1}/{number2}")]
        public IActionResult HelperAdd(int number1, int number2)
        {
            var result = new Helper().Add(number1, number2);
            return Ok(result);
        }
        #endregion
    }
}