using Domain.Entities;
using Domain.Repositories;
using Application.ExternalServices;
using Domain.Exceptions;
using Application.Enums;
using static Application.UseCases.EnrollInCourse.EnrollInCourseResponse;

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
        var student = await _studentRepository.GetByIdAsync(request.StudentId) ?? throw new StudentNotFoundException();
        var course = await _courseRepository.GetByIdAsync(request.CourseId) ?? throw new CourseNotFoundException();

        if (course.Students.Any(s => s.Id == student.Id))
            throw new StudentAlreadyEnrolledException(request.StudentId, request.CourseId);        

        if(IsPaymentRequired(course))
        {
            request.ValidateInfoPayment();
            await EnrrolProcessPaymentAsync(course.Fee.Value.Amount, course.Fee.Value.Currency,request.CallBackUrl,request.PaymentMethod.Value);
            return new EnrollInCourseResponse(EnrrolStatus.PendingPayment);
        }

        course.EnrollStudent(student);
        await _courseRepository.UpdateAsync(course);

        return new EnrollInCourseResponse(EnrrolStatus.Finished);
    }

    public async Task<EnrollInCourseResponse> EnrrolFinishedPaymentAsync(EnrrolInCoursePaymentResultRequest request)
    {
        if (request.PaymentStatus == PaymentStatus.Error)
            return new EnrollInCourseResponse(request.PaymentId, EnrrolStatus.PaymentError, request.ErrorMessage);

        return new EnrollInCourseResponse(request.PaymentId, EnrrolStatus.Finished, request.ErrorMessage);
    }

    private bool IsPaymentRequired(Course course) 
        => course.Fee.Value.Amount > 0;

    private async Task EnrrolProcessPaymentAsync(decimal amount,string currency,string callBackUrl,PaymentMethod paymentMethod)
    {
        try
        {
            var paymentResponse = await _paymentGateway.ProcessPaymentAsync(new ProcessPaymentRequest(amount, currency, callBackUrl, paymentMethod));
        }
        catch (Exception ex)
        {
            throw new PaymentFailedException(ex.Message);
        }
    }
}
