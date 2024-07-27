namespace Domain.Exceptions;

public class InvalidCourseDateRangeException : DomainException
{
    public InvalidCourseDateRangeException() : base("Start date cannot be greater than or equal to end date.") { }
}
