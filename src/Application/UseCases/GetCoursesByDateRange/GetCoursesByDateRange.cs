using Domain.Repositories;
using Application.DTOs;

namespace Application.UseCases.GetCoursesByDateRange;

public class GetCoursesByDateRangeUseCase
{
    private readonly ICourseRepository _courseRepository;

    public GetCoursesByDateRangeUseCase(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<GetCoursesByDateRangeResponse> HandleAsync(GetCoursesByDateRangeRequest request)
    {
        var courses = await _courseRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);

        var courseDtos = courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Name = c.Name.Value,
            Fee = c.Fee.Value.Amount,
            StartDate = c.StartDate.Value,
            EndDate = c.EndDate.Value
        });

        return new GetCoursesByDateRangeResponse(courseDtos);
    }
}
