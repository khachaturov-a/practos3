using Microsoft.EntityFrameworkCore;
using Practos3.Data;
using Practos3.Models;
using Practos3.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена в appsettings.json");

builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IBeadService, BeadService>();

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Магазин чёток API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
    db.Database.Migrate();
}

app.MapGet("/", (IConfiguration config) =>
{
    var appName = config["AppSettings:AppName"] ?? "Магазин чёток";
    var version = config["AppSettings:Version"] ?? "1.0";
    return Results.Ok(new
    {
        application = appName,
        version,
        endpoints = new[]
        {
            "GET  /api/products                     — список всех чёток",
            "GET  /api/products/{id}                — чётки по id",
            "GET  /api/categories                   — список категорий",
            "GET  /api/products/by-category/{id}    — чётки по категории",
            "GET  /api/config                       — конфигурация приложения",
            "POST /api/categories                   — добавить категорию",
            "POST /api/products                     — добавить товар",
            "GET  /swagger                          — Swagger UI"
        }
    });
});

app.MapGet("/api/products", async (IBeadService service, IConfiguration config) =>
{
    var maxItems = int.TryParse(config["AppSettings:MaxItems"], out var max) ? max : 50;
    var items = (await service.GetAllAsync(maxItems)).ToList();
    return Results.Ok(new ProductsResponse(items.Count, items));
})
.WithName("GetProducts")
.WithSummary("Получить список всех чёток")
.Produces<ProductsResponse>();

app.MapGet("/api/products/{id:int}", async (int id, ShopContext db) =>
{
    var item = await db.Chetkas
        .Include(c => c.Category)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (item is null)
        return Results.NotFound(new { message = $"Товар с id={id} не найден" });

    return Results.Ok(new ChetkasDto(
        item.Id, item.Name, item.Price,
        item.StockQuantity, item.Material,
        item.Category?.Name ?? "—"
    ));
})
.WithName("GetProductById")
.WithSummary("Получить чётки по id")
.Produces<ChetkasDto>()
.Produces(404);

app.MapGet("/api/categories", async (IBeadService service) =>
{
    var categories = await service.GetCategoriesAsync();
    return Results.Ok(categories);
})
.WithName("GetCategories")
.WithSummary("Получить список категорий")
.Produces<IEnumerable<CategoryDto>>();

app.MapGet("/api/products/by-category/{categoryId:int}", async (
    int categoryId,
    IBeadService service,
    IConfiguration config) =>
{
    var maxItems = int.TryParse(config["AppSettings:MaxItems"], out var max) ? max : 50;
    var items = await service.GetByCategoryAsync(categoryId, maxItems);
    return Results.Ok(items);
})
.WithName("GetProductsByCategory")
.WithSummary("Получить чётки по категории")
.Produces<IEnumerable<ChetkasDto>>();

app.MapPost("/api/categories", async (Category newCategory, IBeadService service, ShopContext db) =>
{
    var nameExists = await db.Categories.AnyAsync(c => c.Name == newCategory.Name);
    if (nameExists)
        return Results.BadRequest(new { message = $"Категория с именем '{newCategory.Name}' уже существует" });

    var created = await service.CreateCategoryAsync(newCategory);
    return Results.Created($"/api/categories/{created.Id}", created);
})
.WithName("CreateCategory")
.WithSummary("Добавить новую категорию")
.Produces<CategoryDto>(201)
.Produces(400);

app.MapGet("/api/config", (IConfiguration config) =>
{
    return Results.Ok(new
    {
        appName  = config["AppSettings:AppName"],
        version  = config["AppSettings:Version"],
        maxItems = config["AppSettings:MaxItems"],
        dbSource = config.GetConnectionString("DefaultConnection")
    });
})
.WithName("GetConfig")
.WithSummary("Получить конфигурацию приложения")
.Produces<object>();

app.MapPost("/api/products", async (Chetkas newItem, IBeadService service, ShopContext db) =>
{
    var categoryExists = await db.Categories.AnyAsync(c => c.Id == newItem.CategoryId);
    if (!categoryExists)
        return Results.BadRequest(new { message = $"Категория с id={newItem.CategoryId} не существует" });

    var created = await service.CreateAsync(newItem);
    return Results.Created($"/api/products/{created.Id}", created);
})
.WithName("CreateProduct")
.WithSummary("Добавить новый товар")
.Produces<ChetkasDto>(201)
.Produces(400);


app.MapHealthChecks("/health");

app.Run();
