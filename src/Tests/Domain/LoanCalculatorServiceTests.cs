using Domain.Loans.ValueObjects;
using Domain.Loans.Entities;
using Domain.Loans.Enums;
using Domain.Loans.Services;
using FluentAssertions;

namespace Tests.Domain
{
    public class LoanCalculatorServiceTests
    {
        private readonly LoanCalculatorService _calculator = new();

        [Fact]
        public void BuildFrenchSchedule_ShouldReturnCorrectNumberOfInstallments()
        {
            // Arrange
            var loan = Loan.Create(
                clientId: Guid.NewGuid(),
                amount: 10_000,
                annualRate: 0.24m,
                termInMonths: 12,
                type: LoanType.FixedInstallment);

            var firstPaymentDate = new DateTime(2025, 2, 1);

            // Act
            var schedule = _calculator.BuildSchedule(loan, firstPaymentDate);

            // Assert
            schedule.Should().HaveCount(12);
        }

        [Fact]
        public void BuildFrenchSchedule_RemainingBalanceShouldBeZeroAtEnd()
        {
            // Arrange
            var loan = Loan.Create(
                clientId: Guid.NewGuid(),
                amount: 5_000,
                annualRate: 0.18m,
                termInMonths: 6,
                type: LoanType.FixedInstallment);

            // Act
            var schedule = _calculator.BuildSchedule(loan, DateTime.Now.AddMonths(1));

            // Assert — al final no debe quedar saldo
            schedule.Last().RemainingBalance.Amount.Should().Be(0);
        }

        [Fact]
        public void BuildGermanSchedule_PrincipalShouldBeConstant()
        {
            // Arrange
            var loan = Loan.Create(
                clientId: Guid.NewGuid(),
                amount: 12_000,
                annualRate: 0.24m,
                termInMonths: 12,
                type: LoanType.DecreasingInstallment);

            // Act
            var schedule = _calculator.BuildSchedule(loan, DateTime.Now.AddMonths(1));

            // Assert — en sistema alemán el capital es constante
            var principals = schedule
                .Take(schedule.Count - 1) // excluir última cuota
                .Select(s => s.PrincipalAmount.Amount)
                .Distinct();

            principals.Should().HaveCount(1);
        }

        [Fact]
        public void GetNextBusinessDay_ShouldSkipWeekends()
        {
            // Arrange — 2025/03/22 es sábado
            var saturday = new DateTime(2025, 3, 22);

            // Act
            var result = LoanCalculatorService.GetNextBusinessDay(saturday);

            // Assert — debe mover al lunes
            result.DayOfWeek.Should().Be(DayOfWeek.Monday);
        }

        [Fact]
        public void InterestRate_GetRateForPeriod_ShouldApplyDocumentFormula()
        {
            // Arrange — TEA 24%
            var tea = 0.24m;
            var days = 30;

            // Act — fórmula del documento: TEM = [(1 + TEA)^(días/365)] - 1
            var expected = (decimal)(Math.Pow((double)(1 + tea), days / 365.0) - 1);
            var rate = InterestRate.Of(tea);
            var result = rate.GetRateForPeriod(days);

            // Assert
            result.Should().BeApproximately(expected, 0.000001m);
        }
    }
}