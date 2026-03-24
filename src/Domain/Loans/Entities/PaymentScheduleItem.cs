using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Loans.Enums;
using Domain.Loans.ValueObjects;

namespace Domain.Loans.Entities
{
    public class PaymentScheduleItem : Entity
    {
        public Guid LoanId { get; private set; }
        public int InstallmentNumber { get; private set; }
        public DateTime DueDate { get; private set; }         
        public DateTime NominalDueDate { get; private set; }  
        public Money TotalAmount { get; private set; } = null!;
        public Money PrincipalAmount { get; private set; } = null!;
        public Money InterestAmount { get; private set; } = null!;
        public Money RemainingBalance { get; private set; } = null!;
        public decimal EffectiveRate { get; private set; }   
        public int ActualDays { get; private set; }           
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

        private PaymentScheduleItem() { } 

        public static PaymentScheduleItem Create(
            Guid loanId,
            int installmentNumber,
            DateTime nominalDueDate,
            DateTime adjustedDueDate,
            decimal totalAmount,
            decimal principalAmount,
            decimal interestAmount,
            decimal remainingBalance,
            decimal effectiveRate,
            int actualDays)
        {
            return new PaymentScheduleItem
            {
                LoanId = loanId,
                InstallmentNumber = installmentNumber,
                NominalDueDate = nominalDueDate,
                DueDate = adjustedDueDate,
                TotalAmount = Money.Of(totalAmount),
                PrincipalAmount = Money.Of(principalAmount),
                InterestAmount = Money.Of(interestAmount),
                RemainingBalance = Money.Of(remainingBalance),
                EffectiveRate = effectiveRate,
                ActualDays = actualDays
            };
        }
    }
}