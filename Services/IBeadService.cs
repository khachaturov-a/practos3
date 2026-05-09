using Practos3.Models;

namespace Practos3.Services;

public interface IBeadService
{
    Task<IEnumerable<ChetkasDto>> GetAllAsync(int maxItems);
    
    Task<IEnumerable<ChetkasDto>> GetByCategoryAsync(int categoryId, int maxItems);
    
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    
    Task<ChetkasDto> CreateAsync(Chetkas chetkas);
    
    Task<CategoryDto> CreateCategoryAsync(Category category);
}

