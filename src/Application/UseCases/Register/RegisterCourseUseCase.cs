using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;

namespace Application.UseCases.Register;

public class RegisterCourseUseCase
{
    private readonly ICourseRepository _courseRepository;

    public RegisterCourseUseCase(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<RegisterCourseResponse> HandleAsync(RegisterCourseRequest request)
    {
        var courseName = new CourseName(request.Name);
        var courseFee = new CourseFee(new Money(request.Fee, request.Currency));
        var startDate = new CourseStartDate(request.StartDate);
        var endDate = new CourseEndDate(request.EndDate);

        var course = new Course(courseName, courseFee, startDate, endDate);

        await _courseRepository.AddAsync(course);

        return new RegisterCourseResponse(course.Id);
    }
}
