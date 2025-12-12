using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;
using TransactionService.Data;
using TransactionService.Models;

namespace TransactionService.Endpoints;

public static class TransactionEndpoints
{
    public static RouteGroupBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transactions");

        group.MapGet("", async ([AsParameters] TransactionFilter filter, TransactionDbContext dbContext, CancellationToken ct) =>
        {
            if (!filter.TryParse(out var type, out var from, out var to, out var error))
            {
                return Results.BadRequest(error);
            }

            var query = dbContext.Transactions.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.ProductCode))
            {
                var normalized = filter.ProductCode.Trim().ToUpperInvariant();
                query = query.Where(t => t.ProductCode.ToUpper() == normalized);
            }

            if (type.HasValue)
            {
                query = query.Where(t => t.Type == type.Value);
            }

            if (from.HasValue)
            {
                query = query.Where(t => t.PerformedAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(t => t.PerformedAt <= to.Value);
            }

            var items = await query.OrderByDescending(t => t.PerformedAt).ToListAsync(ct);
            return Results.Ok(items);
        });

        group.MapPost("", async (TransactionRequest request, TransactionDbContext dbContext, CancellationToken ct) =>
        {
            if (request.Quantity <= 0)
            {
                return Results.BadRequest("La cantidad debe ser mayor que cero.");
            }

            var normalized = request.ProductCode.Trim().ToUpperInvariant();
            var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Code.ToUpper() == normalized, ct);
            if (product is null)
            {
                return Results.NotFound($"No se encontró el producto con código {request.ProductCode}.");
            }

            if (request.Type == TransactionType.Sale && product.Stock < request.Quantity)
            {
                return Results.BadRequest("No hay stock suficiente para completar la venta.");
            }

            product.Stock += request.Type == TransactionType.Purchase ? request.Quantity : -request.Quantity;

            var transaction = new TransactionRecord
            {
                ProductCode = product.Code,
                Quantity = request.Quantity,
                Type = request.Type,
                PerformedBy = request.PerformedBy,
                Notes = request.Notes,
                PerformedAt = DateTime.UtcNow
            };

            dbContext.Transactions.Add(transaction);
            await dbContext.SaveChangesAsync(ct);

            return Results.Created($"/transactions/{transaction.Id}", transaction);
        });

        group.MapGet("/products/{code}", async (string code, TransactionDbContext dbContext, CancellationToken ct) =>
        {
            var normalized = code.Trim().ToUpperInvariant();
            var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Code.ToUpper() == normalized, ct);
            if (product is null)
            {
                return Results.NotFound();
            }

            var history = await dbContext.Transactions.AsNoTracking()
                .Where(t => t.ProductCode.ToUpper() == normalized)
                .OrderByDescending(t => t.PerformedAt)
                .ToListAsync(ct);

            var result = new
            {
                product.Code,
                product.Name,
                product.Stock,
                Transactions = history
            };

            return Results.Ok(result);
        });

        return group;
    }
}
