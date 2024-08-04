namespace Domain.Exceptions
{
    public class InvalidRangeDatesException : DomainException
    {
        public InvalidRangeDatesException() : base("Start date cannot be after end date.") { }
    }
}
