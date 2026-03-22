using Domain.Common;
using Domain.Loans.Entities;
using Domain.Loans.Repositories;
using Domain.Loans.Services;
using MediatR;

namespace Application.Loans.Commands.ApplyLoan
{
    public class ApplyLoanHandler : IRequestHandler<ApplyLoanCommand, ApplyLoanResponse>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LoanCalculatorService _calculator;

        public ApplyLoanHandler(
            ILoanRepository loanRepository,
            IUnitOfWork unitOfWork,
            LoanCalculatorService calculator)
        {
            _loanRepository = loanRepository;
            _unitOfWork = unitOfWork;
            _calculator = calculator;
        }

        public async Task<ApplyLoanResponse> Handle(
            ApplyLoanCommand request,
            CancellationToken cancellationToken)
        {
            var activeCount = await _loanRepository
                .GetActiveLoanCountAsync(request.ClientId);

            if (activeCount >= 3)
                throw new DomainException("El cliente no puede tener más de 3 préstamos activos");

            var loan = Loan.Create(
                request.ClientId,
                request.Amount,
                request.AnnualRate,
                request.TermInMonths,
                request.Type);

            var schedule = _calculator.BuildSchedule(loan, request.FirstPaymentDate);
            loan.SetPaymentSchedule(schedule);

            loan.Submit();

            await _loanRepository.AddAsync(loan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ApplyLoanResponse(
                loan.Id,
                loan.ClientId,
                loan.Amount.Amount,
                loan.Status,
                loan.CreatedAt);
        }
    }
}