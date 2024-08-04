using Domain.Exceptions;

namespace Application.UseCases.GetCoursesByDateRange;

public class GetCoursesByDateRangeRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public void Validate()
    {
        if (StartDate > EndDate) 
            throw new InvalidRangeDatesException();        
    }
}
