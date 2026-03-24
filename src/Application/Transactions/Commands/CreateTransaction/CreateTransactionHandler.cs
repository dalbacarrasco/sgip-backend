using Domain.Common;
using Domain.Transactions.Entities;
using Domain.Transactions.Repositories;
using MediatR;

namespace Application.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResponse>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTransactionHandler(
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateTransactionResponse> Handle(
            CreateTransactionCommand request,
            CancellationToken cancellationToken)
        {
            var existing = await _transactionRepository
                .GetByIdempotencyKeyAsync(request.IdempotencyKey);

            if (existing is not null)
            {
                return new CreateTransactionResponse(
                    existing.Id,
                    existing.IdempotencyKey,
                    existing.Status,
                    existing.Amount.Amount,
                    WasDuplicate: true);
            }

            var recentTransactions = await _transactionRepository
                .GetRecentByClientIdAsync(request.ClientId, minutes: 2);

            var largAmount = 10_000m;
            var recentLargeCount = recentTransactions
                .Count(t => t.Amount.Amount >= largAmount);

            if (recentLargeCount >= 2)
                throw new DomainException(
                    "Transacción bloqueada: actividad sospechosa detectada");

            var transaction = Transaction.Create(
                request.ClientId,
                request.IdempotencyKey,
                request.Type,
                request.Amount,
                request.ReferenceId,
                request.Description);

            transaction.StartProcessing();
            transaction.Complete();

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateTransactionResponse(
                transaction.Id,
                transaction.IdempotencyKey,
                transaction.Status,
                transaction.Amount.Amount,
                WasDuplicate: false);
        }
    }
}