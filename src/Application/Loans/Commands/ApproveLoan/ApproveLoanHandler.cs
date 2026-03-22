using Domain.Common;
using Domain.Loans.Repositories;
using Domain.Loans.Services;
using MediatR;

namespace Application.Loans.Commands.ApproveLoan
{
    public class ApproveLoanHandler : IRequestHandler<ApproveLoanCommand, bool>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LoanCalculatorService _calculator;

        public ApproveLoanHandler(
            ILoanRepository loanRepository,
            IUnitOfWork unitOfWork,
            LoanCalculatorService calculator)
        {
            _loanRepository = loanRepository;
            _unitOfWork = unitOfWork;
            _calculator = calculator;
        }

        public async Task<bool> Handle(
            ApproveLoanCommand request,
            CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetByIdAsync(request.LoanId)
                ?? throw new DomainException("Préstamo no encontrado");

            loan.StartEvaluation();

            var score = _calculator.CalculateCreditScore(
                onTimePaymentRate: 0.95m,
                debtToIncomeRatio: 0.30m,
                clientAgeInMonths: 24);

            if (_calculator.IsLoanApproved(score))
                loan.Approve();
            else
                loan.Reject($"Score crediticio insuficiente: {score:F1}/100");

            _loanRepository.Update(loan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return loan.Status == Domain.Loans.Enums.LoanStatus.Approved;
        }
    }
}