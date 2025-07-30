using System.ComponentModel.DataAnnotations;

namespace MenuService.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        [Required]
        public string Category { get; set; } = string.Empty; // "Lanche", "Sobremesa", "Bebida"
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}

