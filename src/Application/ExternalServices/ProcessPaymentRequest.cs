using Application.Enums;

namespace Application.ExternalServices
{
    public class ProcessPaymentRequest
    {
        public ProcessPaymentRequest(decimal amount, string currency,string callBackUrl, PaymentMethod paymentMethod)
        {
            PaymentId = Guid.NewGuid();
            Currency = currency;
            PaymentMethod = paymentMethod;
            CallBackUrl = callBackUrl;
        }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CallBackUrl { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
   
}
