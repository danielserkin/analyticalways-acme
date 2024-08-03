namespace Domain.Exceptions
{
    public class NullPaymentMethodException : DomainException
    {
        public NullPaymentMethodException() : base("Payment method cannot be null.") { }
    }
}
