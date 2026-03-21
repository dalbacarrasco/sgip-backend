using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Loans.Entities;
using Domain.Loans.Enums;

namespace Domain.Loans.Services
{
    public class LoanCalculatorService
    {
        private static readonly HashSet<DateTime> _holidays = new()
    {
        new DateTime(2025, 1, 1),   // año nuevo
        new DateTime(2025, 5, 1),   // día del trabajo
        new DateTime(2025, 12, 25), // navidad
    };

        public List<PaymentScheduleItem> BuildSchedule(
            Loan loan,
            DateTime firstPaymentDate)
        {
            return loan.Type switch
            {
                LoanType.FixedInstallment => BuildFrenchSchedule(loan, firstPaymentDate),
                LoanType.DecreasingInstallment => BuildGermanSchedule(loan, firstPaymentDate),
                _ => throw new DomainException("Tipo de préstamo no soportado")
            };
        }

        private List<PaymentScheduleItem> BuildFrenchSchedule(
            Loan loan,
            DateTime firstPaymentDate)
        {
            var schedule = new List<PaymentScheduleItem>();
            var principal = loan.Amount.Amount;
            var tea = loan.InterestRate.AnnualRate;
            var n = loan.TermInMonths;

            var tem = (decimal)(Math.Pow((double)(1 + tea), 30.0 / 365.0) - 1);

            var fixedInstallment = principal * tem / (1 - (decimal)Math.Pow((double)(1 + tem), -n));

            var remainingBalance = principal;
            var previousDate = DateTime.UtcNow;

            for (int i = 1; i <= n; i++)
            {
                var nominalDate = firstPaymentDate.AddMonths(i - 1);
                var adjustedDate = GetNextBusinessDay(nominalDate);
                var actualDays = (adjustedDate - previousDate).Days;

                var periodRate = (decimal)(Math.Pow((double)(1 + tea), actualDays / 365.0) - 1);
                var interestAmount = remainingBalance * periodRate;
                var principalAmount = fixedInstallment - interestAmount;

                if (i == n)
                {
                    principalAmount = remainingBalance;
                    fixedInstallment = principalAmount + interestAmount;
                }

                remainingBalance -= principalAmount;

                schedule.Add(PaymentScheduleItem.Create(
                    loan.Id,
                    i,
                    nominalDate,
                    adjustedDate,
                    Math.Round(principalAmount + interestAmount, 2),
                    Math.Round(principalAmount, 2),
                    Math.Round(interestAmount, 2),
                    Math.Round(Math.Max(remainingBalance, 0), 2),
                    periodRate,
                    actualDays));

                previousDate = adjustedDate;
            }

            return schedule;
        }

        private List<PaymentScheduleItem> BuildGermanSchedule(
            Loan loan,
            DateTime firstPaymentDate)
        {
            var schedule = new List<PaymentScheduleItem>();
            var principal = loan.Amount.Amount;
            var tea = loan.InterestRate.AnnualRate;
            var n = loan.TermInMonths;

            var constantPrincipal = Math.Round(principal / n, 2);
            var remainingBalance = principal;
            var previousDate = DateTime.UtcNow;

            for (int i = 1; i <= n; i++)
            {
                var nominalDate = firstPaymentDate.AddMonths(i - 1);
                var adjustedDate = GetNextBusinessDay(nominalDate);
                var actualDays = (adjustedDate - previousDate).Days;

                var periodRate = (decimal)(Math.Pow((double)(1 + tea), actualDays / 365.0) - 1);
                var interestAmount = remainingBalance * periodRate;

                var principalAmount = i == n ? remainingBalance : constantPrincipal;

                remainingBalance -= principalAmount;

                schedule.Add(PaymentScheduleItem.Create(
                    loan.Id,
                    i,
                    nominalDate,
                    adjustedDate,
                    Math.Round(principalAmount + interestAmount, 2),
                    Math.Round(principalAmount, 2),
                    Math.Round(interestAmount, 2),
                    Math.Round(Math.Max(remainingBalance, 0), 2),
                    periodRate,
                    actualDays));

                previousDate = adjustedDate;
            }

            return schedule;
        }

        public static DateTime GetNextBusinessDay(DateTime date)
        {
            while (date.DayOfWeek == DayOfWeek.Saturday
                   || date.DayOfWeek == DayOfWeek.Sunday
                   || _holidays.Contains(date.Date))
            {
                date = date.AddDays(1);
            }
            return date;
        }

        public decimal CalculateCreditScore(
            decimal onTimePaymentRate, 
            decimal debtToIncomeRatio, 
            int clientAgeInMonths)     
        {
            var historyScore = onTimePaymentRate * 40;                        
            var debtScore = (1 - debtToIncomeRatio) * 35;                      
            var seniorityScore = Math.Min(clientAgeInMonths / 24m, 1) * 25;    

            return historyScore + debtScore + seniorityScore; 
        }

        public bool IsLoanApproved(decimal creditScore) => creditScore >= 60;
    }
}