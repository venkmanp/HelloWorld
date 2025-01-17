﻿using System.ComponentModel.DataAnnotations;

namespace HelloWorld.Entities
{
    public class Category (string name)
    {
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = name;

        public List<Product> Products { get; set; } = new List<Product>();
    }

}
