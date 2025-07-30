using System.ComponentModel.DataAnnotations;

namespace KitchenService.Model
{
	public class KitchenOrder
	{
		public int Id { get; set; }

		[Required]
		public int CustomerId { get; set; }

		[Required]
		public string CustomerName { get; set; } = string.Empty;

		public List<KitchenOrderItem> KitchenOrderItems { get; set; } = new List<KitchenOrderItem>();

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

	public class KitchenOrderItem
	{
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }

		public KitchenOrder KitchenOrder { get; set; } = null!;

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
