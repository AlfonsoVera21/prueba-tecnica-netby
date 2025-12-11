namespace SharedKernel.Domain;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Category { get; set; }
    public ProductType Type { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string NormalizedName => Name.ToUpperInvariant();
    public string NormalizedCode => Code.ToUpperInvariant();
}
