using AutoFixture;
using Domain.Entities;
using Domain.Repositories;
using Application.DTOs;
using Application.UseCases.GetCoursesByDateRange;
using FluentAssertions;
using Moq;
using Domain.Exceptions;


namespace UnitTests.Application.UseCases.GetCoursesByDateRange;

public class GetCoursesByDateRangeUseCaseTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;

    public GetCoursesByDateRangeUseCaseTests()
    {
        _fixture = new Fixture();
        _courseRepositoryMock = new Mock<ICourseRepository>();

        _fixture.Customize<Course>(composer => composer
            .FromFactory(() =>
            {
                var startDate = _fixture.Create<DateTime>();
                var endDate = startDate.AddDays(_fixture.Create<Generator<int>>().First(days => days >= 1 && days <= 365));

                return new Course(
                    _fixture.Create<CourseName>(),
                    _fixture.Create<CourseFee>(),
                    new CourseStartDate(startDate),
                    new CourseEndDate(endDate));
            }));
    }

    [Fact]
    public async Task HandleAsync_WithValidDateRange_ShouldReturnCoursesWithinRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow.AddDays(10);
        var request = new GetCoursesByDateRangeRequest { StartDate = startDate, EndDate = endDate };

        var coursesInRange = _fixture.CreateMany<Course>(5).ToList(); 
        var coursesOutOfRange = _fixture.CreateMany<Course>(3).ToList(); 

        _courseRepositoryMock.Setup(repo => repo.GetByDateRangeAsync(startDate, endDate))
            .ReturnsAsync(coursesInRange);

        var useCase = new GetCoursesByDateRangeUseCase(_courseRepositoryMock.Object);

        // Act
        var response = await useCase.HandleAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Courses.Should().HaveCount(coursesInRange.Count);
        response.Courses.Should().BeEquivalentTo(coursesInRange.Select(c => new CourseDto
        {
            Id = c.Id,
            Name = c.Name.Value,
            Fee = c.Fee.Value.Amount,
            StartDate = c.StartDate.Value,
            EndDate = c.EndDate.Value
        }));

        _courseRepositoryMock.Verify(repo => repo.GetByDateRangeAsync(startDate, endDate), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidDateRange_ShouldThrowInvalidRangeDatesException()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(10);
        var endDate = DateTime.UtcNow; 

        var request = new GetCoursesByDateRangeRequest { StartDate = startDate, EndDate = endDate };
        var useCase = new GetCoursesByDateRangeUseCase(_courseRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRangeDatesException>(() => useCase.HandleAsync(request));
        _courseRepositoryMock.Verify(repo => repo.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
    }

}

