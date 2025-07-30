using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public string DeliveryType { get; set; } = string.Empty; // "Balcão", "Drive-thru", "Delivery"
        
        [Required]
        public string Status { get; set; } = "Pending"; // "Pending", "Accepted", "Rejected", "Preparing", "Ready", "Delivered", "Cancelled"
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public string? CancellationReason { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        public Order Order { get; set; } = null!;
        
        [Required]
        public int MenuItemId { get; set; }
        
        [Required]
        public string MenuItemName { get; set; } = string.Empty;
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
    }
}

