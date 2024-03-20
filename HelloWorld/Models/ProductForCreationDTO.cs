using System.ComponentModel.DataAnnotations;

namespace HelloWorld.Models
{
    public class ProductForCreationDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { set; get; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double Price { get; set; }
    }
}