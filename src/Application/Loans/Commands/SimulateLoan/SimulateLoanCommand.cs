using Domain.Loans.Enums;
using MediatR;

namespace Application.Loans.Commands.SimulateLoan
{
    public record SimulateLoanCommand(
        decimal Amount,
        decimal AnnualRate,
        int TermInMonths,
        LoanType Type,
        DateTime FirstPaymentDate
    ) : IRequest<SimulateLoanResponse>;
}