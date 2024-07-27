using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;

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
        var age = new Age(request.Age);
        var student = new Student(Guid.NewGuid(), request.Name, age);

        await _studentRepository.AddAsync(student);

        return new RegisterStudentResponse(student.Id);
    }
}
