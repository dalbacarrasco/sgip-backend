using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Transactions.Enums;
using Domain.Loans.ValueObjects;
using Domain.Transactions.Events;

namespace Domain.Transactions.Entities
{
    public class Transaction : Entity
    {
        public Guid ClientId { get; private set; }
        public string IdempotencyKey { get; private set; } = null!;
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }
        public Money Amount { get; private set; } = null!;
        public Guid? ReferenceId { get; private set; }
        public string? Description { get; private set; }
        public string? Metadata { get; private set; }
        public string? FailureReason { get; private set; }
        public int RowVersion { get; private set; }

        private Transaction() { }

        public static Transaction Create(
            Guid clientId,
            string idempotencyKey,
            TransactionType type,
            decimal amount,
            Guid? referenceId = null,
            string? description = null,
            string? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new DomainException("La clave de idempotencia es requerida");

            if (amount <= 0)
                throw new DomainException("El monto debe ser mayor a cero");

            var transaction = new Transaction
            {
                ClientId = clientId,
                IdempotencyKey = idempotencyKey,
                Type = type,
                Status = TransactionStatus.Pending,
                Amount = Money.Of(amount),
                ReferenceId = referenceId,
                Description = description,
                Metadata = metadata
            };

            transaction.AddDomainEvent(new TransactionCreatedEvent(
                transaction.Id, clientId, amount, type));

            return transaction;
        }

        public void StartProcessing()
        {
            if (Status != TransactionStatus.Pending)
                throw new DomainException("Solo se pueden procesar transacciones pendientes");

            Status = TransactionStatus.Processing;
            SetUpdated();
        }

        public void Complete()
        {
            if (Status != TransactionStatus.Processing)
                throw new DomainException("La transacción debe estar en procesamiento");

            Status = TransactionStatus.Completed;
            SetUpdated();
            AddDomainEvent(new TransactionCompletedEvent(Id, ClientId, Amount.Amount));
        }

        public void Fail(string reason)
        {
            if (Status == TransactionStatus.Completed)
                throw new DomainException("No se puede fallar una transacción completada");

            Status = TransactionStatus.Failed;
            FailureReason = reason;
            SetUpdated();
        }

        public void Reverse()
        {
            if (Status != TransactionStatus.Completed)
                throw new DomainException("Solo se pueden revertir transacciones completadas");

            Status = TransactionStatus.Reversed;
            SetUpdated();
        }
    }
}