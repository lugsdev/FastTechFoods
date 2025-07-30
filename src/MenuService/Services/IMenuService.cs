using Common.DTOs;

namespace MenuService.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItemDto>> GetAllMenuItemsAsync();
        Task<IEnumerable<MenuItemDto>> GetAvailableMenuItemsAsync();
        Task<IEnumerable<MenuItemDto>> GetMenuItemsByCategoryAsync(string category);
        Task<MenuItemDto?> GetMenuItemByIdAsync(int id);
        Task<MenuItemDto?> CreateMenuItemAsync(CreateMenuItemDto createMenuItemDto);
        Task<MenuItemDto?> UpdateMenuItemAsync(int id, UpdateMenuItemDto updateMenuItemDto);
        Task<bool> DeleteMenuItemAsync(int id);
        Task<IEnumerable<MenuItemDto>> SearchMenuItemsAsync(string searchTerm);
    }
}

