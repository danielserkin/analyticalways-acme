namespace Application.ExternalServices;

public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(decimal amount, string currency);
}
