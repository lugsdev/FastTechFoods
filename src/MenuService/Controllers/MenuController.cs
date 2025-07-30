using Common.DTOs;
using MenuService.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MenuService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly Services.IMenuService _menuService;
        private readonly Publisher _publisher;

        public MenuController(Services.IMenuService menuService, Publisher publisher)
        {
            _menuService = menuService;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAllMenuItems()
        {
            var menuItems = await _menuService.GetAllMenuItemsAsync();
            return Ok(menuItems);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetAvailableMenuItems()
        {
            var menuItems = await _menuService.GetAvailableMenuItemsAsync();
            return Ok(menuItems);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> GetMenuItemsByCategory(string category)
        {
            var menuItems = await _menuService.GetMenuItemsByCategoryAsync(category);
            return Ok(menuItems);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItemById(int id)
        {
            var menuItem = await _menuService.GetMenuItemByIdAsync(id);
            
            if (menuItem == null)
            {
                return NotFound(new { message = "Item do cardápio não encontrado" });
            }

            return Ok(menuItem);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")] // Apenas funcionários podem criar itens
        public async Task<ActionResult<MenuItemDto>> CreateMenuItem([FromBody] CreateMenuItemDto createMenuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menuItem = await _menuService.CreateMenuItemAsync(createMenuItemDto);
            
            if (menuItem == null)
            {
                return BadRequest(new { message = "Erro ao criar item do cardápio" });
            }
            await _publisher.Publish(menuItem);
            return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItem.Id }, menuItem);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")] // Apenas funcionários podem editar itens
        public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(int id, [FromBody] UpdateMenuItemDto updateMenuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var menuItem = await _menuService.UpdateMenuItemAsync(id, updateMenuItemDto);
            
            if (menuItem == null)
            {
                return NotFound(new { message = "Item do cardápio não encontrado" });
            }

			await _publisher.Publish(menuItem);
			return Ok(menuItem);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")] // Apenas funcionários podem deletar itens
        public async Task<ActionResult> DeleteMenuItem(int id)
        {
            var success = await _menuService.DeleteMenuItemAsync(id);
            
            if (!success)
            {
                return NotFound(new { message = "Item do cardápio não encontrado" });
            }

			return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MenuItemDto>>> SearchMenuItems([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Termo de busca é obrigatório" });
            }

            var menuItems = await _menuService.SearchMenuItemsAsync(searchTerm);
            return Ok(menuItems);
        }
    }
}

