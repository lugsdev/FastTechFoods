using Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Messaging;
using OrderService.Models;
using System.Security.Claims;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly Services.IOrderService _orderService;

        public OrderController(Services.IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId) || userId != createOrderDto.CustomerId)
            {
                return Forbid("Você só pode criar pedidos para si mesmo");
            }

			var orderDto = await _orderService.CreateOrderAsync(createOrderDto);

			if (orderDto == null)
            {
                return BadRequest(new { message = "Erro ao criar pedido. Verifique se todos os itens estão disponíveis." });
            }

            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userRole == "Customer" && userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                if (userId != order.CustomerId)
                {
                    return Forbid("Você só pode visualizar seus próprios pedidos");
                }
            }

            return Ok(order);
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomerId(int customerId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userRole == "Customer" && userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                if (userId != customerId)
                {
                    return Forbid("Você só pode visualizar seus próprios pedidos");
                }
            }

            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")] // Apenas funcionários podem ver todos os pedidos
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Employee")] // Apenas funcionários podem filtrar por status
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(string status)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Employee")] // Apenas funcionários podem atualizar status
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.UpdateOrderStatusAsync(id, updateStatusDto);
            
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado" });
            }

            return Ok(order);
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CancelOrder(int id, [FromBody] CancelOrderDto cancelOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (userRole == "Customer" && userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                if (userId != order.CustomerId)
                {
                    return Forbid("Você só pode cancelar seus próprios pedidos");
                }
            }

            if (!await _orderService.CanCancelOrderAsync(id))
            {
                return BadRequest(new { message = "Este pedido não pode ser cancelado" });
            }

            var cancelledOrder = await _orderService.CancelOrderAsync(id, cancelOrderDto);
            
            if (cancelledOrder == null)
            {
                return BadRequest(new { message = "Erro ao cancelar pedido" });
            }

			return Ok(cancelledOrder);
        }
    }
}

