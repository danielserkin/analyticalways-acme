namespace Application.UseCases.Register;

public class RegisterStudentResponse
{
    public Guid StudentId { get; }
    public bool IsSuccess { get; } 
    public string? ErrorMessage { get; } 

    public RegisterStudentResponse(Guid studentId, bool isSuccess = true, string? errorMessage = null)
    {
        StudentId = studentId;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
}
