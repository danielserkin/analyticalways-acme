namespace Domain.Entities;

public class Course : BaseEntity<Guid>
{
    public Course(Guid id, CourseName name, CourseFee fee, CourseStartDate startDate, CourseEndDate endDate)
    {
        Id = id;
        Name = name;
        Fee = fee;
        StartDate = startDate;
        EndDate = endDate;

        if (startDate.Value >= endDate.Value)
        {
            throw new ArgumentException("La fecha de inicio no puede ser igual o posterior a la fecha de fin.");
        }
    }

    public CourseName Name { get; private set; }
    public CourseFee Fee { get; private set; }
    public CourseStartDate StartDate { get; private set; }
    public CourseEndDate EndDate { get; private set; }
}
