using Common.DTOs;
using MenuService.Data;
using MenuService.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuService.Services
{
    public class MenuServiceImpl : IMenuService
    {
        private readonly MenuDbContext _context;

        public MenuServiceImpl(MenuDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return menuItems.Select(MapToMenuItemDto);
        }

        public async Task<IEnumerable<MenuItemDto>> GetAvailableMenuItemsAsync()
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.IsAvailable)
                .ToListAsync();
            return menuItems.Select(MapToMenuItemDto);
        }

        public async Task<IEnumerable<MenuItemDto>> GetMenuItemsByCategoryAsync(string category)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.Category.ToLower() == category.ToLower() && m.IsAvailable)
                .ToListAsync();
            return menuItems.Select(MapToMenuItemDto);
        }

        public async Task<MenuItemDto?> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            return menuItem != null ? MapToMenuItemDto(menuItem) : null;
        }

        public async Task<MenuItemDto?> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto)
        {
            var menuItem = new MenuItem
            {
                Name = createMenuItemDto.Name,
                Description = createMenuItemDto.Description,
                Price = createMenuItemDto.Price,
                IsAvailable = createMenuItemDto.IsAvailable,
                Category = createMenuItemDto.Category,
                CreatedAt = DateTime.UtcNow
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return MapToMenuItemDto(menuItem);
        }

        public async Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto updateMenuItemDto)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(updateMenuItemDto.Name))
                menuItem.Name = updateMenuItemDto.Name;

            if (!string.IsNullOrEmpty(updateMenuItemDto.Description))
                menuItem.Description = updateMenuItemDto.Description;

            if (updateMenuItemDto.Price.HasValue)
                menuItem.Price = updateMenuItemDto.Price.Value;

            if (updateMenuItemDto.IsAvailable.HasValue)
                menuItem.IsAvailable = updateMenuItemDto.IsAvailable.Value;

            if (!string.IsNullOrEmpty(updateMenuItemDto.Category))
                menuItem.Category = updateMenuItemDto.Category;

            menuItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToMenuItemDto(menuItem);
        }

        public async Task<bool> DeleteMenuItemAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return false;
            }

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<MenuItemDto>> SearchMenuItemsAsync(string searchTerm)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.IsAvailable && 
                           (m.Name.ToLower().Contains(searchTerm.ToLower()) ||
                            m.Description.ToLower().Contains(searchTerm.ToLower()) ||
                            m.Category.ToLower().Contains(searchTerm.ToLower())))
                .ToListAsync();

            return menuItems.Select(MapToMenuItemDto);
        }

        private static MenuItemDto MapToMenuItemDto(MenuItem menuItem)
        {
            return new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                Category = menuItem.Category
            };
        }
    }
}

