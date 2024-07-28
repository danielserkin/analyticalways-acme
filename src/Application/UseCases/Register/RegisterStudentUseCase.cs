using Domain.Entities;
using Domain.Repositories;

namespace Application.UseCases.Register;

public class RegisterStudentUseCase
{
    private readonly IStudentRepository _studentRepository;

    public RegisterStudentUseCase(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<RegisterStudentResponse> HandleAsync(RegisterStudentRequest request)
    {
        try
        {
            var student = new Student(request.Name, request.BirthDate);

            await _studentRepository.AddAsync(student);

            return new RegisterStudentResponse(student.Id);
        }
        catch (Exception ex)
        {
            return new RegisterStudentResponse(Guid.Empty, false, ex.Message);
        }

    }
}
