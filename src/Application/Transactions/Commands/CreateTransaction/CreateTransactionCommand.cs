using Domain.Transactions.Enums;
using MediatR;

namespace Application.Transactions.Commands.CreateTransaction
{
    public record CreateTransactionCommand(
         Guid ClientId,
         string IdempotencyKey,
         TransactionType Type,
         decimal Amount,
         Guid? ReferenceId = null,
         string? Description = null
     ) : IRequest<CreateTransactionResponse>;
}