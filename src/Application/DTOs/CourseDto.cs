namespace Application.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Fee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
