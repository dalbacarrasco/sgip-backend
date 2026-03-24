using Domain.Common;
using Domain.Loans.Entities;
using Domain.Loans.Services;
using MediatR;

namespace Application.Loans.Commands.SimulateLoan
{
    public class SimulateLoanHandler : IRequestHandler<SimulateLoanCommand, SimulateLoanResponse>
    {
        private readonly LoanCalculatorService _calculator;

        public SimulateLoanHandler(LoanCalculatorService calculator)
        {
            _calculator = calculator;
        }

        public Task<SimulateLoanResponse> Handle(
            SimulateLoanCommand request,
            CancellationToken cancellationToken)
        {
            var loan = Loan.Create(
                clientId: Guid.Empty,
                amount: request.Amount,
                annualRate: request.AnnualRate,
                termInMonths: request.TermInMonths,
                type: request.Type);

            var schedule = _calculator.BuildSchedule(loan, request.FirstPaymentDate);

            var response = new SimulateLoanResponse(
                Amount: request.Amount,
                AnnualRate: request.AnnualRate,
                TermInMonths: request.TermInMonths,
                FixedInstallment: schedule.First().TotalAmount.Amount,
                Schedule: schedule.Select(s => new PaymentScheduleItemDto(
                    s.InstallmentNumber,
                    s.NominalDueDate,
                    s.DueDate,
                    s.TotalAmount.Amount,
                    s.PrincipalAmount.Amount,
                    s.InterestAmount.Amount,
                    s.RemainingBalance.Amount,
                    s.EffectiveRate,
                    s.ActualDays
                )).ToList()
            );

            return Task.FromResult(response);
        }
    }
}