using SharedKernel.Domain;

namespace ProductService.Models;

public sealed class ProductRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Category { get; init; }
    public ProductType Type { get; init; }
    public decimal UnitPrice { get; init; }
    public int Stock { get; init; }
    public string? Notes { get; init; }
}
