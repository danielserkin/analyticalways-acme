namespace Application.UseCases.EnrollInCourse
{
    public class EnrrolInCoursePaymentResultRequest
    {
        public Guid PaymentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ErrorMessage { get; set; }
        
    }
    public enum PaymentStatus
    {
        Success,
        Error
    }
}
