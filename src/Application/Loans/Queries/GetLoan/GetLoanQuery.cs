using MediatR;

namespace Application.Loans.Queries.GetLoan
{
    public record GetLoanQuery(Guid LoanId) : IRequest<GetLoanResponse?>;
}