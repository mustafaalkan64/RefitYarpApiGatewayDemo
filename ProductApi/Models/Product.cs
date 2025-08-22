using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models
{
    public record Product
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}
