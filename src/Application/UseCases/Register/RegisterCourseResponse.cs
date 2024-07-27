namespace Application.UseCases.Register;

public class RegisterCourseResponse
{
    public Guid CourseId { get; }

    public RegisterCourseResponse(Guid courseId)
    {
        CourseId = courseId;
    }
}
