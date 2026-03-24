using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Loans.Events
{
    public class LoanApprovedEvent : DomainEvent
    {
        public Guid LoanId { get; }
        public Guid ClientId { get; }

        public LoanApprovedEvent(Guid loanId, Guid clientId)
        {
            LoanId = loanId;
            ClientId = clientId;
        }
    }
}