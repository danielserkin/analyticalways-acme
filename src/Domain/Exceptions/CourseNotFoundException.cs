namespace Domain.Exceptions
{
    public class CourseNotFoundException : DomainException
    {
        public CourseNotFoundException() : base($"Course Not found Exception.") { }
    }
}
