using Common.DTOs;
using System.Text.Json;

namespace KitchenService.Services
{
    public class KitchenServiceImpl : IKitchenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KitchenServiceImpl(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<OrderDto>> GetPendingOrdersAsync()
        {
            return await GetOrdersByStatusAsync("Pending");
        }

        public async Task<IEnumerable<OrderDto>> GetAcceptedOrdersAsync()
        {
            return await GetOrdersByStatusAsync("Accepted");
        }

        public async Task<IEnumerable<OrderDto>> GetAllKitchenOrdersAsync()
        {
            try
            {
                var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://localhost:5003";
                
                // Adicionar token de autorização
                AddAuthorizationHeader();
                
                var response = await _httpClient.GetAsync($"{orderServiceUrl}/api/order");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var orders = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Filtrar apenas pedidos relevantes para a cozinha
                    return orders?.Where(o => o.Status != "Cancelled" && o.Status != "Delivered") ?? new List<OrderDto>();
                }
            }
            catch (Exception ex)
            {
                // Log do erro (em um cenário real, usar um logger)
                Console.WriteLine($"Erro ao buscar pedidos: {ex.Message}");
            }

            return new List<OrderDto>();
        }

        public async Task<OrderDto?> AcceptOrderAsync(int orderId)
        {
            return await UpdateOrderStatusAsync(orderId, "Accepted", null);
        }

        public async Task<OrderDto?> RejectOrderAsync(int orderId, string reason)
        {
            return await UpdateOrderStatusAsync(orderId, "Rejected", reason);
        }

        public async Task<OrderDto?> StartPreparingOrderAsync(int orderId)
        {
            return await UpdateOrderStatusAsync(orderId, "Preparing", null);
        }

        public async Task<OrderDto?> FinishOrderAsync(int orderId)
        {
            return await UpdateOrderStatusAsync(orderId, "Ready", null);
        }

        private async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            try
            {
                var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://localhost:5003";
                
                // Adicionar token de autorização
                AddAuthorizationHeader();
                
                var response = await _httpClient.GetAsync($"{orderServiceUrl}/api/order/status/{status}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<OrderDto>();
                }
            }
            catch (Exception ex)
            {
                // Log do erro (em um cenário real, usar um logger)
                Console.WriteLine($"Erro ao buscar pedidos por status {status}: {ex.Message}");
            }

            return new List<OrderDto>();
        }

        private async Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string status, string? reason)
        {
            try
            {
                var orderServiceUrl = _configuration["Services:OrderService"] ?? "http://localhost:5003";
                
                var updateDto = new UpdateOrderStatusDto
                {
                    Status = status,
                    Reason = reason
                };

                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Adicionar token de autorização
                AddAuthorizationHeader();

                var response = await _httpClient.PutAsync($"{orderServiceUrl}/api/order/{orderId}/status", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OrderDto>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                // Log do erro (em um cenário real, usar um logger)
                Console.WriteLine($"Erro ao atualizar status do pedido {orderId}: {ex.Message}");
            }

            return null;
        }

        private void AddAuthorizationHeader()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);
            }
        }
    }
}

