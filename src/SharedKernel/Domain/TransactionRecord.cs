namespace SharedKernel.Domain;

public class TransactionRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string ProductCode { get; set; }
    public int Quantity { get; set; }
    public TransactionType Type { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
