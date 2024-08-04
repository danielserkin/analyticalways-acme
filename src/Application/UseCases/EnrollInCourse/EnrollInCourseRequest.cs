using Application.Enums;
using Domain.Exceptions;

namespace Application.UseCases.EnrollInCourse;

public class EnrollInCourseRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? CallBackUrl { get; set; }
    public void ValidateInfoPayment()
    {
        if (PaymentMethod == null)
            throw new NullPaymentMethodException();        

        if (string.IsNullOrEmpty(CallBackUrl))
            throw new NullOrEmptyCallBackUrlException();
    }
}
