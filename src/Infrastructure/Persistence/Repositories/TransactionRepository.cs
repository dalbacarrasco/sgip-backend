using Domain.Transactions.Entities;
using Domain.Transactions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(Guid id) =>
            await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey) =>
            await _context.Transactions
                .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);

        public async Task<List<Transaction>> GetByClientIdAsync(Guid clientId) =>
            await _context.Transactions
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

        public async Task<List<Transaction>> GetRecentByClientIdAsync(
            Guid clientId, int minutes) =>
            await _context.Transactions
                .Where(t => t.ClientId == clientId
                    && t.CreatedAt >= DateTime.UtcNow.AddMinutes(-minutes))
                .ToListAsync();

        public async Task AddAsync(Transaction transaction) =>
            await _context.Transactions.AddAsync(transaction);

        public void Update(Transaction transaction) =>
            _context.Transactions.Update(transaction);
    }
}