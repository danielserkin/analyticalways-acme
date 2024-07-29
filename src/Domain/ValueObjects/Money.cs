namespace Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("The amount cannot be negative.");       

        Amount = amount;
        Currency = currency?? throw new ArgumentNullException(nameof(currency));
    }
}