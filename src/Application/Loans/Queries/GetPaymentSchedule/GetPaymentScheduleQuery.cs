using Application.Loans.Commands.SimulateLoan;
using MediatR;

namespace Application.Loans.Queries.GetPaymentSchedule
{
    public record GetPaymentScheduleQuery(Guid LoanId) : IRequest<List<PaymentScheduleItemDto>>
    {
        public record PaymentScheduleItemDto(
            int InstallmentNumber,
            DateTime NominalDueDate,
            DateTime DueDate,
            decimal TotalAmount,
            decimal PrincipalAmount,
            decimal InterestAmount,
            decimal RemainingBalance,
            decimal EffectiveRate,
            int ActualDays,
            string Status
        );
    }
}