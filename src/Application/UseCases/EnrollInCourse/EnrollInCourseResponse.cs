namespace Application.UseCases.EnrollInCourse;

public class EnrollInCourseResponse
{
    public bool Success { get; }
    public string Message { get; }

    public EnrollInCourseResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}
