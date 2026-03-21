using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Loans.Enums;
using Domain.Loans.ValueObjects;
using Domain.Loans.Events;

namespace Domain.Loans.Entities
{
    public class Loan : Entity
    {
        public Guid ClientId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public InterestRate InterestRate { get; private set; } = null!;
        public int TermInMonths { get; private set; }
        public LoanType Type { get; private set; }
        public LoanStatus Status { get; private set; }
        public DateTime? DisbursementDate { get; private set; }
        public string? RejectionReason { get; private set; }
        public int RowVersion { get; private set; } 

        private readonly List<PaymentScheduleItem> _paymentSchedule = new();
        public IReadOnlyCollection<PaymentScheduleItem> PaymentSchedule =>
            _paymentSchedule.AsReadOnly();

        private Loan() { } 

        public static Loan Create(
            Guid clientId,
            decimal amount,
            decimal annualRate,
            int termInMonths,
            LoanType type)
        {
            if (amount < 500 || amount > 50_000)
                throw new DomainException("El monto debe estar entre $500 y $50,000");

            if (termInMonths < 6 || termInMonths > 60)
                throw new DomainException("El plazo debe estar entre 6 y 60 meses");

            var loan = new Loan
            {
                ClientId = clientId,
                Amount = Money.Of(amount),
                InterestRate = InterestRate.Of(annualRate),
                TermInMonths = termInMonths,
                Type = type,
                Status = LoanStatus.Draft
            };

            loan.AddDomainEvent(new LoanCreatedEvent(loan.Id, clientId, amount));
            return loan;
        }

        public void Submit()
        {
            if (Status != LoanStatus.Draft)
                throw new DomainException("Solo se puede enviar un préstamo en borrador");

            Status = LoanStatus.PendingApproval;
            SetUpdated();
        }

        public void StartEvaluation()
        {
            if (Status != LoanStatus.PendingApproval)
                throw new DomainException("El préstamo debe estar pendiente de aprobación");

            Status = LoanStatus.Evaluating;
            SetUpdated();
        }

        public void Approve()
        {
            if (Status != LoanStatus.Evaluating)
                throw new DomainException("El préstamo debe estar en evaluación para aprobarse");

            Status = LoanStatus.Approved;
            SetUpdated();
            AddDomainEvent(new LoanApprovedEvent(Id, ClientId));
        }

        public void Reject(string reason)
        {
            if (Status != LoanStatus.Evaluating)
                throw new DomainException("El préstamo debe estar en evaluación para rechazarse");

            Status = LoanStatus.Rejected;
            RejectionReason = reason;
            SetUpdated();
        }

        public void Disburse()
        {
            if (Status != LoanStatus.Approved)
                throw new DomainException("El préstamo debe estar aprobado para desembolsarse");

            Status = LoanStatus.Disbursed;
            DisbursementDate = DateTime.UtcNow;
            SetUpdated();
        }

        public void Activate()
        {
            if (Status != LoanStatus.Disbursed)
                throw new DomainException("El préstamo debe estar desembolsado para activarse");

            Status = LoanStatus.Active;
            SetUpdated();
        }

        public void SetPaymentSchedule(List<PaymentScheduleItem> schedule)
        {
            _paymentSchedule.Clear();
            _paymentSchedule.AddRange(schedule);
        }
    }
}