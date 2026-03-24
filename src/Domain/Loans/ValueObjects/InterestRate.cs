using Domain.Common;
namespace Domain.Loans.ValueObjects;

public sealed class InterestRate : ValueObject
{
    public decimal AnnualRate { get; }

    private InterestRate() { }

    private InterestRate(decimal annualRate)
    {
        if (annualRate < 0.18m || annualRate > 0.35m)
            throw new DomainException("La TEA debe estar entre 18% y 35%");

        AnnualRate = annualRate;
    }

    public static InterestRate Of(decimal annualRate) => new(annualRate);
    
    public decimal GetRateForPeriod(int days) =>
        (decimal)(Math.Pow((double)(1 + AnnualRate), days / 365.0) - 1);

    public decimal AsPercentage => AnnualRate * 100;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AnnualRate;
    }

    public override string ToString() => $"{AsPercentage:F2}%";
}