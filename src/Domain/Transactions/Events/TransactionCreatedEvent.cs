using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Transactions.Enums;

namespace Domain.Transactions.Events
{
    public class TransactionCreatedEvent : DomainEvent
    {
        public Guid TransactionId { get; }
        public Guid ClientId { get; }
        public decimal Amount { get; }
        public TransactionType Type { get; }

        public TransactionCreatedEvent(
            Guid transactionId,
            Guid clientId,
            decimal amount,
            TransactionType type)
        {
            TransactionId = transactionId;
            ClientId = clientId;
            Amount = amount;
            Type = type;
        }
    }
}