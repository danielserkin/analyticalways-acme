namespace Application.UseCases.EnrollInCourse;

public class EnrollInCourseResponse
{
    public EnrollInCourseResponse(Guid paymentId,EnrrolStatus status, string paymentErrorMessage)
    {
        PaymentId = paymentId;
        Status = status;
        PaymentErrorMessage = paymentErrorMessage;
    }

    public EnrollInCourseResponse(EnrrolStatus status) => Status = status;

    public Guid? PaymentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid StudentId { get; private set; }
    public EnrrolStatus Status { get; private set; }
    public string PaymentErrorMessage { get; private set; }

    public enum EnrrolStatus
    {
        Finished,
        PendingPayment,
        PaymentError
    }
}
