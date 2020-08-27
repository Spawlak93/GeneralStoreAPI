using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Product
    { 
        public Product()
        {
            SKU = GenerateSKU();
        }
        [Key]
        public string SKU { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public int NumberInInventory { get; set; }
        public bool IsInStock => NumberInInventory > 0;

        private string GenerateSKU()
        {
            Random random = new Random();

            var sKU = random.Next(100000).ToString("D5");

            return $"GS--{sKU}";
        }
    }
}