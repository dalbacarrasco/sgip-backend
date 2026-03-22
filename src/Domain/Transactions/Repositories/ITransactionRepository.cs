using Domain.Transactions.Entities;

namespace Domain.Transactions.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id);
        Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<List<Transaction>> GetByClientIdAsync(Guid clientId);
        Task<List<Transaction>> GetRecentByClientIdAsync(Guid clientId, int minutes);
        Task AddAsync(Transaction transaction);
        void Update(Transaction transaction);
    }
}