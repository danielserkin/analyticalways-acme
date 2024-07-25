namespace Domain.Entities;

public class Course : BaseEntity<Guid>
{
    public Course(Guid id, string name, decimal fee, DateTime startDate, DateTime endDate)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Fee = fee;
        StartDate = startDate;
        EndDate = endDate;
        
        if (startDate >= endDate)
        {
            throw new ArgumentException("");
        }
    }
    
    public string Name { get; private set; }
    public decimal Fee { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
}
