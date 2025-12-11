using SharedKernel.Domain;

namespace TransactionService.Models;

public sealed class TransactionFilter
{
    public string? ProductCode { get; init; }
    public TransactionType? Type { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}
