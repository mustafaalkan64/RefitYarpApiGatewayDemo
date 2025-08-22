using Microsoft.AspNetCore.Mvc;
using ConsumerApi.Services;
using ConsumerApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductProxyController : ControllerBase
    {
        private readonly IProductApi _productApi;

        public ProductProxyController(IProductApi productApi)
        {
            _productApi = productApi;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = await _productApi.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productApi.GetProduct(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            var created = await _productApi.PostProduct(product);
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }
    }
}
