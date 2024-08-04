using Domain.ValueObjects;

public record CourseFee
{
    public Money Value { get; }

    public CourseFee(Money value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        Value = value;
    }
}