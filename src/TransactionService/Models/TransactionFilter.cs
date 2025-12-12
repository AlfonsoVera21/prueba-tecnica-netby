using SharedKernel.Domain;

namespace TransactionService.Models;

public sealed class TransactionFilter
{
    public string? ProductCode { get; init; }
    public string? Type { get; init; }
    public string? From { get; init; }
    public string? To { get; init; }

    public bool TryParse(out TransactionType? parsedType, out DateTime? parsedFrom, out DateTime? parsedTo, out string? error)
    {
        parsedType = null;
        parsedFrom = null;
        parsedTo = null;
        error = null;

        if (!string.IsNullOrWhiteSpace(Type) && !Enum.TryParse(Type, true, out TransactionType transactionType))
        {
            error = "El tipo de transacción es inválido. Usa Purchase o Sale.";
            return false;
        }

        parsedType = string.IsNullOrWhiteSpace(Type) ? null : transactionType;

        if (!string.IsNullOrWhiteSpace(From) && !DateTime.TryParse(From, out DateTime fromDate))
        {
            error = "La fecha 'From' es inválida. Usa un formato ISO 8601 (por ejemplo 2024-01-15).";
            return false;
        }

        parsedFrom = string.IsNullOrWhiteSpace(From) ? null : fromDate;

        if (!string.IsNullOrWhiteSpace(To) && !DateTime.TryParse(To, out DateTime toDate))
        {
            error = "La fecha 'To' es inválida. Usa un formato ISO 8601 (por ejemplo 2024-01-31).";
            return false;
        }

        parsedTo = string.IsNullOrWhiteSpace(To) ? null : toDate;
        return true;
    }
}
