using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Endpoints;
using ProductService.Services;
using SharedKernel.Domain;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "data");
Directory.CreateDirectory(dataDirectory);
var databasePath = Path.Combine(dataDirectory, "netby.db");

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}")
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddScoped<ProductSearchService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await EnsureDatabaseAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProductEndpoints();

app.Run();

static async Task EnsureDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Products.AnyAsync())
    {
        var demoProducts = new List<Product>
        {
            new() { Code = "PRD-001", Name = "Laptop de desarrollo", Category = "Tecnologia", Type = ProductType.Asset, UnitPrice = 1200m, Stock = 10 },
            new() { Code = "PRD-002", Name = "Licencia IDE", Category = "Software", Type = ProductType.Service, UnitPrice = 199.99m, Stock = 50 },
            new() { Code = "PRD-003", Name = "Mouse ergonómico", Category = "Tecnologia", Type = ProductType.Consumer, UnitPrice = 49.90m, Stock = 120 },
            new() { Code = "PRD-004", Name = "Silla gamer", Category = "Muebles", Type = ProductType.Consumer, UnitPrice = 320m, Stock = 25 }
        };

        db.Products.AddRange(demoProducts);
        await db.SaveChangesAsync();
    }
}
