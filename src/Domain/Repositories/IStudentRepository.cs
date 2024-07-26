using Domain.Entities;

namespace Domain.Repositories;

public interface IStudentRepository
{
    Task AddAsync(Student student);
}
