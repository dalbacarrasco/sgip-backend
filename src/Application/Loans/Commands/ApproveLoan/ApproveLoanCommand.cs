using MediatR;

namespace Application.Loans.Commands.ApproveLoan
{
    public record ApproveLoanCommand(Guid LoanId) : IRequest<bool>;
}