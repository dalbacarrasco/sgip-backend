using Domain.Loans.Repositories;
using MediatR;

namespace Application.Loans.Queries.GetLoan
{
    public class GetLoanHandler : IRequestHandler<GetLoanQuery, GetLoanResponse?>
    {
        private readonly ILoanRepository _loanRepository;

        public GetLoanHandler(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<GetLoanResponse?> Handle(
            GetLoanQuery request,
            CancellationToken cancellationToken)
        {
            var loan = await _loanRepository.GetByIdAsync(request.LoanId);

            if (loan is null) return null;

            return new GetLoanResponse(
                loan.Id,
                loan.ClientId,
                loan.Amount.Amount,
                loan.InterestRate.AnnualRate,
                loan.TermInMonths,
                loan.Type,
                loan.Status,
                loan.RejectionReason,
                loan.CreatedAt);
        }
    }
}