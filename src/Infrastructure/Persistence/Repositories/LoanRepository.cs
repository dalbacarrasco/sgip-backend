using Domain.Loans.Entities;
using Domain.Loans.Enums;
using Domain.Loans.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;

        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Loan?> GetByIdAsync(Guid id) =>
            await _context.Loans
                .Include(l => l.PaymentSchedule)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task<List<Loan>> GetByClientIdAsync(Guid clientId) =>
            await _context.Loans
                .Where(l => l.ClientId == clientId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

        public async Task<int> GetActiveLoanCountAsync(Guid clientId) =>
            await _context.Loans
                .CountAsync(l => l.ClientId == clientId
                    && l.Status == LoanStatus.Active);

        public async Task AddAsync(Loan loan) =>
            await _context.Loans.AddAsync(loan);

        public void Update(Loan loan) =>
            _context.Loans.Update(loan);
    }
}