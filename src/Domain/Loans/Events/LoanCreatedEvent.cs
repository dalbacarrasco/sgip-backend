using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Loans.Events
{
    public class LoanCreatedEvent : DomainEvent
    {
        public Guid LoanId { get; }
        public Guid ClientId { get; }
        public decimal Amount { get; }

        public LoanCreatedEvent(Guid loanId, Guid clientId, decimal amount)
        {
            LoanId = loanId;
            ClientId = clientId;
            Amount = amount;
        }
    }
}