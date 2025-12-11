using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Services;
using SharedKernel.Domain;
using SharedKernel.Search;

namespace ProductService.Endpoints;

public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        group.MapGet("", async (ProductSearchService searchService, [AsParameters] ProductSearchFilters filters, CancellationToken ct) =>
        {
            var results = await searchService.SearchAsync(filters, ct);
            return Results.Ok(results);
        });

        group.MapGet("/{code}", async (string code, ProductDbContext dbContext, CancellationToken ct) =>
        {
            var normalized = code.Trim().ToUpperInvariant();
            var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Code.ToUpper() == normalized, ct);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });

        group.MapPost("", async (ProductRequest request, ProductDbContext dbContext, CancellationToken ct) =>
        {
            var normalized = request.Code.Trim().ToUpperInvariant();
            var exists = await dbContext.Products.AnyAsync(p => p.Code.ToUpper() == normalized, ct);
            if (exists)
            {
                return Results.Conflict($"El código {request.Code} ya existe.");
            }

            var product = new Product
            {
                Code = request.Code.Trim(),
                Name = request.Name.Trim(),
                Category = request.Category?.Trim(),
                Type = request.Type,
                UnitPrice = request.UnitPrice,
                Stock = request.Stock
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(ct);

            return Results.Created($"/products/{product.Code}", product);
        });

        group.MapPut("/{code}", async (string code, ProductRequest request, ProductDbContext dbContext, CancellationToken ct) =>
        {
            var normalized = code.Trim().ToUpperInvariant();
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Code.ToUpper() == normalized, ct);
            if (product is null)
            {
                return Results.NotFound();
            }

            product.Name = request.Name.Trim();
            product.Category = request.Category?.Trim();
            product.Type = request.Type;
            product.UnitPrice = request.UnitPrice;
            product.Stock = request.Stock;

            await dbContext.SaveChangesAsync(ct);
            return Results.Ok(product);
        });

        group.MapDelete("/{code}", async (string code, ProductDbContext dbContext, CancellationToken ct) =>
        {
            var normalized = code.Trim().ToUpperInvariant();
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Code.ToUpper() == normalized, ct);
            if (product is null)
            {
                return Results.NotFound();
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        group.MapPost("/{code}/stock", async (string code, StockUpdateRequest request, ProductDbContext dbContext, CancellationToken ct) =>
        {
            var normalized = code.Trim().ToUpperInvariant();
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Code.ToUpper() == normalized, ct);
            if (product is null)
            {
                return Results.NotFound();
            }

            if (request.Delta < 0 && product.Stock + request.Delta < 0)
            {
                return Results.BadRequest("Stock insuficiente para la operación solicitada.");
            }

            product.Stock += request.Delta;
            await dbContext.SaveChangesAsync(ct);
            return Results.Ok(product);
        });

        return group;
    }
}

public record StockUpdateRequest(int Delta);
