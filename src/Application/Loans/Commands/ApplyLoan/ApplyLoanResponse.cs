using Domain.Loans.Enums;

namespace Application.Loans.Commands.ApplyLoan
{
    public record ApplyLoanResponse(
        Guid LoanId,
        Guid ClientId,
        decimal Amount,
        LoanStatus Status,
        DateTime CreatedAt
    );
}