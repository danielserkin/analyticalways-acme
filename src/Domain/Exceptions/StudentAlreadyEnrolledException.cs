namespace Domain.Exceptions;

public class StudentAlreadyEnrolledException : DomainException
{
    public StudentAlreadyEnrolledException(Guid studentId, Guid courseId)
        : base($"Student with ID {studentId} is already enrolled in course with ID {courseId}.") { }
}
