using Domain.Loans.Enums;
using MediatR;

namespace Application.Loans.Commands.ApplyLoan
{
    public record ApplyLoanCommand(
        Guid ClientId,
        decimal Amount,
        decimal AnnualRate,
        int TermInMonths,
        LoanType Type,
        DateTime FirstPaymentDate
    ) : IRequest<ApplyLoanResponse>;
}