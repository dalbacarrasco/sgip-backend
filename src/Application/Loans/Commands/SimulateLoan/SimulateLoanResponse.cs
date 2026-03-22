namespace Application.Loans.Commands.SimulateLoan;

public record SimulateLoanResponse(
    decimal Amount,
    decimal AnnualRate,
    int TermInMonths,
    decimal FixedInstallment,
    List<PaymentScheduleItemDto> Schedule
);

public record PaymentScheduleItemDto(
    int InstallmentNumber,
    DateTime NominalDueDate,
    DateTime DueDate,
    decimal TotalAmount,
    decimal PrincipalAmount,
    decimal InterestAmount,
    decimal RemainingBalance,
    decimal EffectiveRate,
    int ActualDays
);