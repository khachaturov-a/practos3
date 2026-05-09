namespace Practos3.Models;

// DTO для чёток
public record ChetkasDto(
    int    Id,
    string Name,
    decimal Price,
    int    StockQuantity,
    string Material,
    string CategoryName
);

// DTO для категории
public record CategoryDto(
    int    Id,
    string Name,
    string? Description,
    int    ProductCount
);

// Обёртка
public record ProductsResponse(
    int    TotalCount,
    IEnumerable<ChetkasDto> Items
);

