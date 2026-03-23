using Application.Transactions.Commands.CreateTransaction;
using Domain.Common;
using Domain.Transactions.Entities;
using Domain.Transactions.Enums;
using Domain.Transactions.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.Application
{
    public class CreateTransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepoMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly CreateTransactionHandler _handler;

        public CreateTransactionHandlerTests()
        {
            _handler = new CreateTransactionHandler(
                _transactionRepoMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnExisting_WhenIdempotencyKeyAlreadyExists()
        {
            // Arrange
            var idempotencyKey = "pago-123";
            var existingTransaction = Transaction.Create(
                Guid.NewGuid(),
                idempotencyKey,
                TransactionType.InstallmentPayment,
                500);

            // Simular que ya existe en la BD
            _transactionRepoMock
                .Setup(r => r.GetByIdempotencyKeyAsync(idempotencyKey))
                .ReturnsAsync(existingTransaction);

            var command = new CreateTransactionCommand(
                ClientId: Guid.NewGuid(),
                IdempotencyKey: idempotencyKey,
                Type: TransactionType.InstallmentPayment,
                Amount: 500);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert — devuelve la existente y marca como duplicado
            result.WasDuplicate.Should().BeTrue();
            result.TransactionId.Should().Be(existingTransaction.Id);

            // Nunca debió guardar nada nuevo
            _transactionRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCreateNew_WhenIdempotencyKeyIsNew()
        {
            // Arrange
            _transactionRepoMock
                .Setup(r => r.GetByIdempotencyKeyAsync(It.IsAny<string>()))
                .ReturnsAsync((Transaction?)null);

            _transactionRepoMock
                .Setup(r => r.GetRecentByClientIdAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Transaction>());

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new CreateTransactionCommand(
                ClientId: Guid.NewGuid(),
                IdempotencyKey: "pago-nuevo-456",
                Type: TransactionType.InstallmentPayment,
                Amount: 500);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.WasDuplicate.Should().BeFalse();
            result.Status.Should().Be(TransactionStatus.Completed);

            _transactionRepoMock.Verify(
                r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }
    }
}