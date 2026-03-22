using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Transactions.Events
{
    public class TransactionCompletedEvent : DomainEvent
    {
        public Guid TransactionId { get; }
        public Guid ClientId { get; }
        public decimal Amount { get; }

        public TransactionCompletedEvent(
            Guid transactionId,
            Guid clientId,
            decimal amount)
        {
            TransactionId = transactionId;
            ClientId = clientId;
            Amount = amount;
        }
    }
}