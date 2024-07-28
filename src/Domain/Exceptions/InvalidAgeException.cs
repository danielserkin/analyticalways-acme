namespace Domain.Exceptions;

public class InvalidAgeException : DomainException
{
    public InvalidAgeException(int age) : base($"Age must be 18 or older, but was {age}.") { }
}
