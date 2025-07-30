using Common.DTOs;
using OrderService.Data;
using OrderService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using OrderService.Messaging;

namespace OrderService.Services
{
    public class OrderServiceImpl : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
		private readonly Publisher _publisher;

		public OrderServiceImpl(OrderDbContext context, HttpClient httpClient, IConfiguration configuration, Publisher publisher)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
            _publisher = publisher;
        }

        public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto createOrderDto)
        {

            var menuItems = new List<MenuItemDto>();
            foreach (var item in createOrderDto.Items)
            {
                var menuItem = await GetMenuItemAsync(item.MenuItemId);
                if (menuItem == null || !menuItem.IsAvailable)
                {
                    return null;
                }
                menuItems.Add(menuItem);
            }

            var customer = await GetCustomerAsync(createOrderDto.CustomerId);
            if (customer == null)
            {
                return null;
            }

            var order = new Order
            {
                CustomerId = createOrderDto.CustomerId,
                CustomerName = customer.Name,
                DeliveryType = createOrderDto.DeliveryType,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            foreach (var createItem in createOrderDto.Items)
            {
                var menuItem = menuItems.First(m => m.Id == createItem.MenuItemId);
                var totalPrice = menuItem.Price * createItem.Quantity;

                var orderItem = new OrderItem
                {
                    MenuItemId = createItem.MenuItemId,
                    MenuItemName = menuItem.Name,
                    Quantity = createItem.Quantity,
                    UnitPrice = menuItem.Price,
                    TotalPrice = totalPrice
                };

                order.Items.Add(orderItem);
                totalAmount += totalPrice;
            }

            order.TotalAmount = totalAmount;
            //_context.Orders.Add(order);
            await _publisher.Publish(MapToOrderDto(order));
            //await _context.SaveChangesAsync();

            return MapToOrderDto(order);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order != null ? MapToOrderDto(order) : null;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToOrderDto);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToOrderDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapToOrderDto);
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto updateStatusDto)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            order.Status = updateStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(updateStatusDto.Reason))
            {
                order.CancellationReason = updateStatusDto.Reason;
            }

            await _context.SaveChangesAsync();

            return MapToOrderDto(order);
        }

        public async Task<OrderDto?> CancelOrderAsync(int id, CancelOrderDto cancelOrderDto)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return null;
            }

            if (!await CanCancelOrderAsync(id))
            {
                return null;
            }

            order.Status = "Cancelled";
            order.CancellationReason = cancelOrderDto.Reason;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToOrderDto(order);
        }

        public async Task<bool> CanCancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            return order.Status == "Pending" || order.Status == "Accepted";
        }

        private async Task<MenuItemDto?> GetMenuItemAsync(int menuItemId)
        {
            try
            {
                var menuServiceUrl = _configuration["Services:MenuService"] ?? "http://localhost:5002";
                var response = await _httpClient.GetAsync($"{menuServiceUrl}/api/menu/{menuItemId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<MenuItemDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch
            {
                
            }

            return null;
        }

        private async Task<UserDto?> GetCustomerAsync(int customerId)
        {
            try
            {
                var authServiceUrl = _configuration["Services:AuthService"] ?? "http://localhost:5001";
                var response = await _httpClient.GetAsync($"{authServiceUrl}/api/auth/user/{customerId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch
            {
            
            }

            return null;
        }

        private static OrderDto MapToOrderDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    MenuItemId = item.MenuItemId,
                    MenuItemName = item.MenuItemName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList(),
                TotalAmount = order.TotalAmount,
                DeliveryType = order.DeliveryType,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                CancellationReason = order.CancellationReason
            };
        }
    }
}

