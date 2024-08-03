using AutoFixture;
using Domain.Entities;
using Domain.Repositories;
using Application.UseCases.Register;
using FluentAssertions;
using Moq;

namespace Tests.Application.UseCases
{
    public class RegisterStudentUseCaseTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IStudentRepository> _studentRepositoryMock;

        public RegisterStudentUseCaseTests()
        {
            _fixture = new Fixture();
            _studentRepositoryMock = new Mock<IStudentRepository>();
        }

        [Fact]
        public async Task HandleAsync_WithValidRequest_ShouldRegisterStudent()
        {
            // Arrange
            var request = _fixture.Build<RegisterStudentRequest>()
               .With(r => r.BirthDate, GenerateValidBirthDate())
               .Create();

            var studentRepositoryMock = new Mock<IStudentRepository>();

            // Configurar el mock para asignar el ID al estudiante
            studentRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Student>()))
                .Returns(Task.CompletedTask);

            var useCase = new RegisterStudentUseCase(studentRepositoryMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.IsSuccess.Should().BeTrue();
            response.ErrorMessage.Should().BeNull();

            studentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Student>()), Times.Once);
        }



        [Fact]
        public async Task HandleAsync_WithInvalidAge_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = _fixture.Build<RegisterStudentRequest>()
            .With(r => r.BirthDate, DateTime.Now)
            .Create();
            var useCase = new RegisterStudentUseCase(_studentRepositoryMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Contain("Age must be 18 or older");
            _studentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Student>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRepositoryThrowsException_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = _fixture.Build<RegisterStudentRequest>()
            .With(r => r.BirthDate, GenerateValidBirthDate())
            .Create();

            var exceptionMessage = "Repository error";
            _studentRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Student>()))
                .ThrowsAsync(new Exception(exceptionMessage));
            var useCase = new RegisterStudentUseCase(_studentRepositoryMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Be(exceptionMessage);
        }

        [Fact]
        public async Task HandleAsync_WithNullName_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new RegisterStudentRequest
            {
                Name = null,
                BirthDate = _fixture.Create<DateTime>()
            };
            var useCase = new RegisterStudentUseCase(_studentRepositoryMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessage.Should().Contain("Value cannot be null.");
            _studentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Student>()), Times.Never);
        }
        private DateTime GenerateValidBirthDate()
        {
            var today = DateTime.Today;
            var maxBirthDate = today.AddYears(-18);

            var start = new DateTime(1900, 1, 1);
            var range = (maxBirthDate - start).Days;
            return start.AddDays(_fixture.Create<Random>().Next(range));
        }
    }
}
