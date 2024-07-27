namespace Application.UseCases.EnrollInCourse;

public class EnrollInCourseRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
}
