using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Kalem" },
            new Product { Id = 2, Name = "Notebook" },
            new Product { Id = 3, Name = "Kitap" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> PostProduct(Product product)
        {
            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
    }
}