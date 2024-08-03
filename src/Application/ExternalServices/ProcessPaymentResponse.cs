namespace Application.ExternalServices
{
    public class ProcessPaymentResponse
    {
        public bool Success { get; set; }

        public Guid PaymentId { get; set; }
    }
}
