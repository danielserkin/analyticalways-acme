using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Student : BaseEntity<Guid>
{
    public Student(string name, DateTime birthDate)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ValidateBirthDate(birthDate);
        BirthDate = birthDate;
    }

    public string Name { get; private set; }
    public DateTime BirthDate { get; private set; }
    public int Age => new Age(BirthDate).Years;

    private void ValidateBirthDate(DateTime birthDate)
    {
        var age = new Age(birthDate);
        if (age.Years < 18)
            throw new InvalidAgeException(age.Years);        
    }
}
