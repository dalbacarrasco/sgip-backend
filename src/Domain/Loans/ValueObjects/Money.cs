using Domain.Common;
namespace Domain.Loans.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public  string Currency { get; }

    private Money() { }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("El monto no puede ser negativo");

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpper();
    }

    public static Money Of(decimal amount, string currency = "USD") =>
        new(amount, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("No se pueden sumar monedas distintas");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("No se pueden restar monedas distintas");
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) =>
        new(Amount * factor, Currency);

    public bool IsGreaterThan(Money other) => Amount > other.Amount;
    public bool IsLessThan(Money other) => Amount < other.Amount;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:F2}";
}