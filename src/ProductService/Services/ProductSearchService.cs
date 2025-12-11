using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using SharedKernel.Domain;
using SharedKernel.Search;

namespace ProductService.Services;

public class ProductSearchService
{
    private readonly ProductDbContext _dbContext;

    public ProductSearchService(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(ProductSearchFilters filters, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Category))
        {
            var normalizedCategory = filters.Category.Trim().ToUpperInvariant();
            query = query.Where(p => p.Category != null && p.Category.ToUpperInvariant() == normalizedCategory);
        }

        if (filters.Type.HasValue)
        {
            query = query.Where(p => p.Type == filters.Type);
        }

        if (filters.MinPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.UnitPrice <= filters.MaxPrice.Value);
        }

        var items = await query.ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            items = ApplyBinarySearch(items, p => p.NormalizedName, filters.Name);
        }

        if (!string.IsNullOrWhiteSpace(filters.Code))
        {
            items = ApplyBinarySearch(items, p => p.NormalizedCode, filters.Code);
        }

        return items.OrderBy(p => p.Name).ToList();
    }

    private static List<Product> ApplyBinarySearch(List<Product> items, Func<Product, string> selector, string prefix)
    {
        var ordered = items.OrderBy(selector).ToList();
        var range = BinarySearchHelper.FindPrefixRange(ordered, selector, prefix.Trim());

        if (range.Start == -1)
        {
            return new List<Product>();
        }

        return ordered.GetRange(range.Start, range.End - range.Start + 1);
    }
}
