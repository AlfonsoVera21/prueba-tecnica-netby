using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;
using TransactionService.Data;
using TransactionService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Postgres")
                       ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
                       ?? "Host=localhost;Port=5432;Database=netby;Username=postgres;Password=postgres";

builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(connectionString)
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await EnsureDatabaseAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapTransactionEndpoints();

app.Run();

static async Task EnsureDatabaseAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Products.AnyAsync())
    {
        var baseProducts = new List<Product>
        {
            new() { Code = "PRD-001", Name = "Laptop de desarrollo", Category = "Tecnologia", Type = ProductType.Asset, UnitPrice = 1200m, Stock = 10 },
            new() { Code = "PRD-002", Name = "Licencia IDE", Category = "Software", Type = ProductType.Service, UnitPrice = 199.99m, Stock = 50 }
        };

        db.Products.AddRange(baseProducts);
        await db.SaveChangesAsync();
    }
}
