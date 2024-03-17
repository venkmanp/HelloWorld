using System.ComponentModel.DataAnnotations;

namespace HelloWorld.Models
{
    public class ProductForUpdateDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { set; get; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double Price { get; set; }
    }
}