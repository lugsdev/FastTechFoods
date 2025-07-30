using Common.DTOs;

namespace KitchenService.Services
{
    public interface IKitchenService
    {
        Task<IEnumerable<OrderDto>> GetPendingOrdersAsync();
        Task<IEnumerable<OrderDto>> GetAcceptedOrdersAsync();
        Task<IEnumerable<OrderDto>> GetAllKitchenOrdersAsync();
        Task<OrderDto?> AcceptOrderAsync(int orderId);
        Task<OrderDto?> RejectOrderAsync(int orderId, string reason);
        Task<OrderDto?> StartPreparingOrderAsync(int orderId);
        Task<OrderDto?> FinishOrderAsync(int orderId);
    }
}

