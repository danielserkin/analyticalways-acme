using Domain.Entities;

namespace Domain.Repositories;

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByIdAsync(Guid id);
    Task UpdateAsync(Course course);
    Task<IEnumerable<Course>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}
