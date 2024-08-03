using AutoFixture;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Application.UseCases.Register;
using FluentAssertions;
using Moq;

namespace Tests.Application.UseCases
{
    public class RegisterCourseUseCaseTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<ICourseRepository> _courseRepositoryMock;

        public RegisterCourseUseCaseTests()
        {
            _fixture = new Fixture();
            _fixture.Customize<RegisterCourseRequest>(composer => composer
                .With(r => r.StartDate, DateTime.UtcNow.AddDays(1))
                .With(r => r.EndDate, DateTime.UtcNow.AddDays(30)));

            _courseRepositoryMock = new Mock<ICourseRepository>();
            _courseRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Course>()))
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task HandleAsync_WithValidRequest_ShouldRegisterCourse()
        {
            // Arrange
            var request = _fixture.Create<RegisterCourseRequest>();
            var courseId = Guid.NewGuid();
            _courseRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Course>()))
                .Returns(Task.CompletedTask);

            var useCase = new RegisterCourseUseCase(_courseRepositoryMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.Should().NotBeNull();
            _courseRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WithInvalidDateRange_ShouldThrowInvalidCourseDateRangeException()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(30);
            var endDate = DateTime.UtcNow;

            var request = new RegisterCourseRequest
            {
                Name = _fixture.Create<string>(),
                Fee = _fixture.Create<decimal>(),
                Currency = _fixture.Create<string>(),
                StartDate = startDate,
                EndDate = endDate
            };

            var useCase = new RegisterCourseUseCase(_courseRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCourseDateRangeException>(() => useCase.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WithInvalidCourseName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var request = _fixture.Build<RegisterCourseRequest>()
                .Create();
            request.Name = null;

            var useCase = new RegisterCourseUseCase(_courseRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => useCase.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WithNegativeFee_ShouldThrowArgumentException()
        {
            // Arrange
            var request = _fixture.Build<RegisterCourseRequest>()
                .With(r => r.Fee, -100m).Create();

            var useCase = new RegisterCourseUseCase(_courseRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => useCase.HandleAsync(request));
        }

        [Fact]
        public async Task HandleAsync_WithInvalidCurrency_ShouldThrowArgumentNullException()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            var request = new RegisterCourseRequest
            {
                Name = _fixture.Create<string>(),
                Fee = _fixture.Create<decimal>(),
                Currency = null,
                StartDate = startDate,
                EndDate = endDate
            };

            var useCase = new RegisterCourseUseCase(_courseRepositoryMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => useCase.HandleAsync(request));
        }
    }
}
