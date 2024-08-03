using Domain.Exceptions;

namespace Domain.Entities;

public class Course : BaseEntity<Guid>
{

    private readonly List<Student> _students = new();

    public Course(CourseName name, CourseFee fee, CourseStartDate startDate, CourseEndDate endDate)
    {
        Id = Guid.NewGuid();
        Name = name;
        Fee = fee;
        StartDate = startDate;
        EndDate = endDate;

        if (startDate.Value.Date >= endDate.Value.Date)
            throw new InvalidCourseDateRangeException(); 
        
    }

    public CourseName Name { get; private set; }
    public CourseFee Fee { get; private set; }
    public CourseStartDate StartDate { get; private set; }
    public CourseEndDate EndDate { get; private set; }
    public IReadOnlyCollection<Student> Students => _students.AsReadOnly();
    public void EnrollStudent(Student student)
    {
        if (_students.Any(s => s.Id == student.Id)) 
            throw new StudentAlreadyEnrolledException(student.Id, Id); 
        
        _students.Add(student);
    }
}
