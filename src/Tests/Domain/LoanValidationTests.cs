using Domain.Common;
using Domain.Loans.Entities;
using Domain.Loans.Enums;
using FluentAssertions;

namespace Tests.Domain
{
    public class LoanValidationTests
    {
        [Fact]
        public void Create_ShouldThrow_WhenAmountBelowMinimum()
        {
            // Arrange & Act
            var act = () => Loan.Create(
                Guid.NewGuid(),
                amount: 100, // menor a $500
                annualRate: 0.24m,
                termInMonths: 12,
                type: LoanType.FixedInstallment);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*$500*");
        }

        [Fact]
        public void Create_ShouldThrow_WhenAmountAboveMaximum()
        {
            var act = () => Loan.Create(
                Guid.NewGuid(),
                amount: 100_000, // mayor a $50,000
                annualRate: 0.24m,
                termInMonths: 12,
                type: LoanType.FixedInstallment);

            act.Should().Throw<DomainException>()
                .WithMessage("*$50,000*");
        }

        [Fact]
        public void Create_ShouldThrow_WhenTermBelowMinimum()
        {
            var act = () => Loan.Create(
                Guid.NewGuid(),
                amount: 5_000,
                annualRate: 0.24m,
                termInMonths: 3, // menor a 6
                type: LoanType.FixedInstallment);

            act.Should().Throw<DomainException>()
                .WithMessage("*6*");
        }

        [Fact]
        public void Approve_ShouldThrow_WhenNotInEvaluatingState()
        {
            // Arrange — préstamo en Draft
            var loan = Loan.Create(
                Guid.NewGuid(), 5_000, 0.24m, 12,
                LoanType.FixedInstallment);

            // Act — intentar aprobar sin pasar por evaluación
            var act = () => loan.Approve();

            // Assert
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void StateMachine_ShouldFollowCorrectFlow()
        {
            // Arrange
            var loan = Loan.Create(
                Guid.NewGuid(), 5_000, 0.24m, 12,
                LoanType.FixedInstallment);

            // Act & Assert — flujo completo del documento
            loan.Status.Should().Be(LoanStatus.Draft);

            loan.Submit();
            loan.Status.Should().Be(LoanStatus.PendingApproval);

            loan.StartEvaluation();
            loan.Status.Should().Be(LoanStatus.Evaluating);

            loan.Approve();
            loan.Status.Should().Be(LoanStatus.Approved);
        }
    }
}