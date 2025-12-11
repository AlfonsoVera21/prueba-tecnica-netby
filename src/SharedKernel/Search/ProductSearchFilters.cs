using SharedKernel.Domain;

namespace SharedKernel.Search;

public sealed class ProductSearchFilters
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Category { get; set; }
    public ProductType? Type { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
