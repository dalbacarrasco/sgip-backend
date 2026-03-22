using Domain.Loans.Entities;

namespace Domain.Loans.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(Guid id);
        Task<List<Loan>> GetByClientIdAsync(Guid clientId);
        Task<int> GetActiveLoanCountAsync(Guid clientId);
        Task AddAsync(Loan loan);
        void Update(Loan loan);
    }
}