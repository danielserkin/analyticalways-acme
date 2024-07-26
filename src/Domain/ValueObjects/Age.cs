namespace Domain.ValueObjects;

public record Age
{
    public int Years { get; }

    public Age(int years)
    {
        if (years < 18)
        {
            throw new ArgumentException("La edad debe ser mayor o igual a 18 años.");
        }

        Years = years;
    }
}