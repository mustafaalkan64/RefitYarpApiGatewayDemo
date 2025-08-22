using Refit;
using ConsumerApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerApi.Services
{
    public interface IProductApi
    {
        [Get("/api/products")]
        Task<List<Product>> GetProducts();

        [Get("/api/products/{id}")]
        Task<Product> GetProduct(int id);

        [Post("/api/products")]
        Task<Product> PostProduct([Body] Product product);
    }
}
