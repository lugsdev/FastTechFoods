using System.ComponentModel.DataAnnotations;

namespace Common.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; } = string.Empty; // "Lanche", "Sobremesa", "Bebida"
    }

    public class CreateMenuItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Price { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        [Required]
        public string Category { get; set; } = string.Empty;
    }

    public class UpdateMenuItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Category { get; set; }
    }
}

