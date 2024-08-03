namespace Domain.Exceptions
{
    public class NullOrEmptyCallBackUrlException : DomainException
    {
        public NullOrEmptyCallBackUrlException() : base($"Callback URL cannot be null or empty.") { }
    }
}

