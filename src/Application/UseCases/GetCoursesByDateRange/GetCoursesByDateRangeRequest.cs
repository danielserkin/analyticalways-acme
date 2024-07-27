namespace Application.UseCases.GetCoursesByDateRange;

public class GetCoursesByDateRangeRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
