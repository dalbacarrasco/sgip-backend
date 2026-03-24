using Domain.Transactions.Enums;

namespace Application.Transactions.Commands.CreateTransaction
{
    public record CreateTransactionResponse(
        Guid TransactionId,
        string IdempotencyKey,
        TransactionStatus Status,
        decimal Amount,
        bool WasDuplicate
    );
}