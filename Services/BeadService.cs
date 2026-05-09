using Microsoft.EntityFrameworkCore;
using Practos3.Data;
using Practos3.Models;

namespace Practos3.Services;

public class BeadService : IBeadService
{
    private readonly ShopContext _db;
    
    public BeadService(ShopContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ChetkasDto>> GetAllAsync(int maxItems)
    {
        var items = await _db.Chetkas
            .Take(maxItems)
            .Select(c => new ChetkasDto(
                c.Id, c.Name, c.Price, c.StockQuantity, c.Material,
                c.Category != null ? c.Category.Name : "—"
            ))
            .ToListAsync();
        // Сортировка товаров по цене
        return items.OrderBy(c => c.Price);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        return await _db.Categories
            .Select(cat => new CategoryDto(
                cat.Id,
                cat.Name,
                cat.Description,
                cat.Chetkas.Count
            ))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ChetkasDto>> GetByCategoryAsync(int categoryId, int maxItems)
    {
        var items = await _db.Chetkas
            .Where(c => c.CategoryId == categoryId)
            .Take(maxItems)
            .Select(c => new ChetkasDto(
                c.Id, c.Name, c.Price, c.StockQuantity, c.Material,
                c.Category != null ? c.Category.Name : "—"
            ))
            .ToListAsync();
        return items.OrderBy(c => c.Price);
    }

    public async Task<ChetkasDto> CreateAsync(Chetkas chetkas)
    {
        _db.Chetkas.Add(chetkas);
        await _db.SaveChangesAsync();
        await _db.Entry(chetkas).Reference(c => c.Category).LoadAsync();

        return new ChetkasDto(chetkas.Id, chetkas.Name, chetkas.Price,
            chetkas.StockQuantity, chetkas.Material, chetkas.Category?.Name ?? "—");
    }

    public async Task<CategoryDto> CreateCategoryAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return new CategoryDto(category.Id, category.Name, category.Description, 0);
    }
}

