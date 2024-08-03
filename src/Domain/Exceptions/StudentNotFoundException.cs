namespace Domain.Exceptions
{
    public class StudentNotFoundException : DomainException
    {
        public StudentNotFoundException() : base($"Student Not found Exception.") { }
    }
}
