using Domain.ValueObjects;

namespace Domain.Entities;

public class Student : BaseEntity<Guid>
{
    public Student(Guid id, string name, Age age)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }

    public string Name { get; private set; }
    public Age Age { get; private set; }

}
