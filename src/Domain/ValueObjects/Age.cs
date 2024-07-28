using Domain.Exceptions;

namespace Domain.ValueObjects;

public record Age
{
    public int Years { get; }

    public Age(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        Years = age;
    }
}