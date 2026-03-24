using Application.Loans.Commands.SimulateLoan;
using Domain.Common;
using Domain.Loans.Repositories;
using MediatR;

namespace Application.Loans.Queries.GetPaymentSchedule
{
    public class GetPaymentScheduleHandler : IRequestHandler<GetPaymentScheduleQuery, List<PaymentScheduleItemDto>>
    {
        private readonly ILoanRepository _loanRepository;

        public GetPaymentScheduleHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<List<PaymentScheduleItemDto>> Handle(
            GetPaymentScheduleQuery request,
            CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetByIdAsync(request.LoanId)
                ?? throw new DomainException("Préstamo no encontrado");

            return loan.PaymentSchedule
                .OrderBy(p => p.InstallmentNumber)
                .Select(p => new PaymentScheduleItemDto(
                    p.InstallmentNumber,
                    p.NominalDueDate,
                    p.DueDate,
                    p.TotalAmount.Amount,
                    p.PrincipalAmount.Amount,
                    p.InterestAmount.Amount,
                    p.RemainingBalance.Amount,
                    p.EffectiveRate,
                    p.ActualDays)
                )
                .ToList();
        }
    }
}