namespace Application.UseCases.Register;

public class RegisterCourseRequest
{
    public string Name { get; set; }
    public decimal Fee { get; set; }
    public string Currency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
