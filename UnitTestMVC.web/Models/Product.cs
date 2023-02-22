using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace UnitTestMVC.web.Models
{
    public class Product
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public int? Stock { get; set; }
        [Required]
        public string? Color { get; set; }
    }
}
