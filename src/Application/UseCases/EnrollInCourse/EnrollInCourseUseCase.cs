using Domain.Entities;
using Domain.Repositories;
using Application.UseCases.EnrollInCourse;
using Application.ExternalServices;
using Domain.Exceptions;

namespace Application.UseCases.EnrollInCourse;

public class EnrollInCourseUseCase
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IPaymentGateway _paymentGateway; 

    public EnrollInCourseUseCase(
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        IPaymentGateway paymentGateway)
    {
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<EnrollInCourseResponse> HandleAsync(EnrollInCourseRequest request)
    {
        try
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId);
            if (student == null) return new EnrollInCourseResponse(false, "Student not found.");

            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null) return new EnrollInCourseResponse(false, "Course not found.");

            if (course.Students.Any(s => s.Id == student.Id))
                return new EnrollInCourseResponse(false, "Student is already enrolled in this course.");

            if (course.Fee.Value.Amount > 0)
            {
                bool paymentSuccessful = await _paymentGateway.ProcessPaymentAsync(course.Fee.Value.Amount, course.Fee.Value.Currency);
                if (!paymentSuccessful) return new EnrollInCourseResponse(false, "Payment failed.");
            }

            course.EnrollStudent(student);
            await _courseRepository.UpdateAsync(course);

            return new EnrollInCourseResponse(true, "Student enrolled successfully.");
        }
        catch (StudentAlreadyEnrolledException ex)
        {
            return new EnrollInCourseResponse(false, ex.Message);
        }
        catch (InvalidCourseDateRangeException ex)
        {
            return new EnrollInCourseResponse(false, ex.Message);
        }

    }
}
