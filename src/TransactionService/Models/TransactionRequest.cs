using SharedKernel.Domain;

namespace TransactionService.Models;

public sealed class TransactionRequest
{
    public required string ProductCode { get; init; }
    public int Quantity { get; init; }
    public TransactionType Type { get; init; }
    public string? PerformedBy { get; init; }
    public string? Notes { get; init; }
}
