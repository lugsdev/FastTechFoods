using Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")] // Apenas funcionários podem acessar a cozinha
    public class KitchenController : ControllerBase
    {
        private readonly Services.IKitchenService _kitchenService;

        public KitchenController(Services.IKitchenService kitchenService)
        {
            _kitchenService = kitchenService;
        }

        [HttpGet("orders/pending")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetPendingOrders()
        {
            var orders = await _kitchenService.GetPendingOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders/accepted")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAcceptedOrders()
        {
            var orders = await _kitchenService.GetAcceptedOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllKitchenOrders()
        {
            var orders = await _kitchenService.GetAllKitchenOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("orders/{orderId}/accept")]
        public async Task<ActionResult<OrderDto>> AcceptOrder(int orderId)
        {
            var order = await _kitchenService.AcceptOrderAsync(orderId);
            
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado ou não pode ser aceito" });
            }
            //await _publisher.ExecuteTask(order);
            return Ok(order);
        }

        [HttpPut("orders/{orderId}/reject")]
        public async Task<ActionResult<OrderDto>> RejectOrder(int orderId, [FromBody] RejectOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(new { message = "Motivo da rejeição é obrigatório" });
            }

            var order = await _kitchenService.RejectOrderAsync(orderId, request.Reason);
            
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado ou não pode ser rejeitado" });
            }

			//await _publisher.Publish(order);
			return Ok(order);
        }

        [HttpPut("orders/{orderId}/start-preparing")]
        public async Task<ActionResult<OrderDto>> StartPreparingOrder(int orderId)
        {
            var order = await _kitchenService.StartPreparingOrderAsync(orderId);
            
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado ou não pode iniciar preparo" });
            }

			//await _publisher.Publish(order);
			return Ok(order);
        }

        [HttpPut("orders/{orderId}/finish")]
        public async Task<ActionResult<OrderDto>> FinishOrder(int orderId)
        {
            var order = await _kitchenService.FinishOrderAsync(orderId);
            
            if (order == null)
            {
                return NotFound(new { message = "Pedido não encontrado ou não pode ser finalizado" });
            }

			//await _publisher.Publish(order);
			return Ok(order);
        }
    }

    public class RejectOrderRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}

