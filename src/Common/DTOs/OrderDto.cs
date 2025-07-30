using System.ComponentModel.DataAnnotations;

namespace Common.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
		public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public decimal TotalAmount { get; set; }
        public string DeliveryType { get; set; } = string.Empty; // "Balcão", "Drive-thru", "Delivery"
        public string Status { get; set; } = string.Empty; // "Pending", "Accepted", "Rejected", "Preparing", "Ready", "Delivered", "Cancelled"
        public DateTime CreatedAt { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class OrderItemDto
    {
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
        
        [Required]
        public string DeliveryType { get; set; } = string.Empty;
    }

    public class CreateOrderItemDto
    {
        [Required]
        public int MenuItemId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
        
        public string? Reason { get; set; }
    }

    public class CancelOrderDto
    {
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}

