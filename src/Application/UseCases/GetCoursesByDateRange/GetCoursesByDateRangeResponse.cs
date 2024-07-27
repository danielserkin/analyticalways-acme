// GetCoursesByDateRangeResponse.cs
using Application.DTOs;

namespace Application.UseCases.GetCoursesByDateRange;

public class GetCoursesByDateRangeResponse
{
    public IEnumerable<CourseDto> Courses { get; }

    public GetCoursesByDateRangeResponse(IEnumerable<CourseDto> courses)
    {
        Courses = courses;
    }
}
