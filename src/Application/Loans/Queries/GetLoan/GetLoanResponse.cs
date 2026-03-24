using Domain.Loans.Enums;

namespace Application.Loans.Queries.GetLoan
{
    public record GetLoanResponse(
        Guid Id,
        Guid ClientId,
        decimal Amount,
        decimal AnnualRate,
        int TermInMonths,
        LoanType Type,
        LoanStatus Status,
        string? RejectionReason,
        DateTime CreatedAt
    );
}