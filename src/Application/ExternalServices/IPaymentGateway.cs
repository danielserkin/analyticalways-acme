namespace Application.ExternalServices;

public interface IPaymentGateway
{
    Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request);
}
