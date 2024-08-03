namespace Domain.Exceptions
{
    public class PaymentFailedException : DomainException
    {
        public PaymentFailedException(string msg) : base($"The payment has failed: {msg}") { }
    }
}
