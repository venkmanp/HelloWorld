﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloWorld.Entities
{

    public class Product (string name)
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = name;

        [MaxLength(500)]
        public string? Description { get; set; }


      
        [Range(1, double.MaxValue)]
        public double Price
        {
            get; set;
        }

        Category? Category { get; set; }

        public int CategoryID { get; set; }
    }

}
