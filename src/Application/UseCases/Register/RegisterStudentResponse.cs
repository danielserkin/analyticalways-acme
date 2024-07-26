namespace Application.UseCases.Register;

public class RegisterStudentResponse
{
    public Guid StudentId { get; }

    public RegisterStudentResponse(Guid studentId)
    {
        StudentId = studentId;
    }
}
