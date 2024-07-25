namespace Domain.Entities;

public  class Student : BaseEntity<Guid>
{
    public Student(Guid id, string name, DateTime birthDate)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BirthDate = birthDate;        
    }
    public string Name { get; private set; }
    public DateTime BirthDate { get; private set; }
}
