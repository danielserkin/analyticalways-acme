namespace Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("El monto no puede ser negativo.");
        }

        Amount = amount;
        Currency = currency;
    }
}