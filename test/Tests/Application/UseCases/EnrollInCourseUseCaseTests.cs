using AutoFixture;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Domain.ValueObjects;
using Application.ExternalServices;
using Application.UseCases.EnrollInCourse;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Application.UseCases.Register;
using Application.Enums;

namespace UnitTests.Application.UseCases.EnrollInCourse
{
    public class EnrollInCourseUseCaseTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IPaymentGateway> _paymentGatewayMock;

        public EnrollInCourseUseCaseTests()
        {
            _fixture = new Fixture();
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _paymentGatewayMock = new Mock<IPaymentGateway>();

            _fixture.Customize<CourseStartDate>(composer => composer
                .FromFactory(() => new CourseStartDate(_fixture.Create<DateTime>())));
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

        private DateTime GenerateValidBirthDate() 
            => DateTime.Today.AddYears(-20);

        [Fact]
        public async Task HandleAsync_WithValidRequestAndFreeCourse_ShouldEnrollStudent()
        {
            // Arrange
            var request = _fixture.Create<EnrollInCourseRequest>();

            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Perez", GenerateValidBirthDate()))
                .Create();

            var course = _fixture.Build<Course>()
                .FromFactory(() => new Course(
                    new CourseName("Test Course"),
                    new CourseFee(new Money(0, "USD")),
                    new CourseStartDate(DateTime.UtcNow.AddDays(1)),
                    new CourseEndDate(DateTime.UtcNow.AddDays(30))
                ))
                .Create();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);
            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(course)).Returns(Task.CompletedTask);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.PaymentId.Should().BeNull();
            course.Students.Should().Contain(student);
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        }


        [Fact]
        public async Task HandleAsync_WithNonExistentStudent_ShouldThrowStudentNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<EnrollInCourseRequest>();
            var course = _fixture.Create<Course>();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync((Student)null);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<StudentNotFoundException>(() => useCase.HandleAsync(request));
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNonExistentCourse_ShouldThrowCourseNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<EnrollInCourseRequest>();
            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Perez", GenerateValidBirthDate()))
                .Create();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync((Course)null);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<CourseNotFoundException>(() => useCase.HandleAsync(request));
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithStudentAlreadyEnrolled_ShouldThrowStudentAlreadyEnrolledException()
        {
            // Arrange
            var request = _fixture.Create<EnrollInCourseRequest>();
            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Doe", GenerateValidBirthDate()))
                .Create();
            var course = _fixture.Build<Course>()
                .FromFactory(() => new Course(
                    new CourseName("Test Course"),
                    _fixture.Create<CourseFee>(),
                    new CourseStartDate(DateTime.UtcNow.AddDays(1)),
                    new CourseEndDate(DateTime.UtcNow.AddDays(30))
                ))
                .Create();
            
            course.EnrollStudent(student);

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<StudentAlreadyEnrolledException>(() => useCase.HandleAsync(request));
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithValidRequestAndPaidCourse_ShouldEnrollStudentAndProcessPayment()
        {
            // Arrange
            var request = _fixture.Create<EnrollInCourseRequest>();
            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Perez", GenerateValidBirthDate()))
                .Create();
            var courseFee = _fixture.Create<decimal>();
            var course = _fixture.Build<Course>()
                .FromFactory(() => new Course(
                    new CourseName("Test Course"),
                    new CourseFee(new Money(courseFee, "USD")),
                    new CourseStartDate(DateTime.UtcNow.AddDays(1)),
                    new CourseEndDate(DateTime.UtcNow.AddDays(30))
                ))
                .Create();
            var paymentId = Guid.NewGuid();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);
            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(course)).Returns(Task.CompletedTask);

            var paymentResponse = new ProcessPaymentResponse();
            paymentResponse.Success = true;
            _paymentGatewayMock.Setup(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()))
                .ReturnsAsync(paymentResponse);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act
            var response = await useCase.HandleAsync(request);

            // Assert
            response.Should().NotBeNull();
            response.Status.Should().Be(EnrollInCourseResponse.EnrrolStatus.PendingPayment);
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Once);
        }


        [Fact]
        public async Task HandleAsync_WithValidRequestAndPaidCourse_ButPaymentFails_ShouldThrowPaymentFailedException()
        {
            // Arrange
            var request = _fixture.Create<EnrollInCourseRequest>();
            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Perez", GenerateValidBirthDate()))
                .Create();
            var courseFee = _fixture.Create<decimal>();
            var course = _fixture.Build<Course>()
                .FromFactory(() => new Course(
                    new CourseName("Test Course"),
                    new CourseFee(new Money(courseFee, "USD")),
                    new CourseStartDate(DateTime.UtcNow.AddDays(1)),
                    new CourseEndDate(DateTime.UtcNow.AddDays(30))
                ))
                .Create();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);

            _paymentGatewayMock.Setup(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()))
                .ThrowsAsync(new Exception("Payment failed"));

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<PaymentFailedException>(() => useCase.HandleAsync(request));
            course.Students.Should().NotContain(student); 
        }

        [Fact]
        public async Task HandleAsync_WithInvalidPaymentMethod_ShouldThrowNullPaymentMethodException()
        {
            // Arrange
            var request = _fixture.Build<EnrollInCourseRequest>()
                .With(r => r.PaymentMethod, (PaymentMethod?)null) 
                .Create();
            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Perez", GenerateValidBirthDate()))
                .Create();

           
            var courseFee = _fixture.Create<decimal>();
            var course = _fixture.Build<Course>()
                .FromFactory(() => new Course(
                    new CourseName("Test Course"),
                    new CourseFee(new Money(courseFee, "USD")),
                    new CourseStartDate(DateTime.UtcNow.AddDays(1)),
                    new CourseEndDate(DateTime.UtcNow.AddDays(30))
                ))
                .Create();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullPaymentMethodException>(() => useCase.HandleAsync(request)); 
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WithNullOrEmptyCallBackUrl_ShouldThrowNullOrEmptyCallbackUrlException()
        {
            // Arrange         
            var request = _fixture.Build<EnrollInCourseRequest>()
                 .With(r => r.CallBackUrl, (string)null)
                 .With(r => r.PaymentMethod, PaymentMethod.CreditCard) 
                 .Create();

            var student = _fixture.Build<Student>()
                .FromFactory(() => new Student("John Doe", GenerateValidBirthDate()))
                .Create();

            var courseFee = _fixture.Create<decimal>();
            var course = _fixture.Build<Course>()    
                .FromFactory(() => new Course(
                    new CourseName("Test Course"),
                    new CourseFee(new Money(courseFee, "USD")),
                    new CourseStartDate(DateTime.UtcNow.AddDays(1)),
                    new CourseEndDate(DateTime.UtcNow.AddDays(30))
                ))
                .Create();

            _studentRepositoryMock.Setup(repo => repo.GetByIdAsync(request.StudentId)).ReturnsAsync(student);
            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(request.CourseId)).ReturnsAsync(course);

            var useCase = new EnrollInCourseUseCase(_studentRepositoryMock.Object, _courseRepositoryMock.Object, _paymentGatewayMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullOrEmptyCallBackUrlException>(() => useCase.HandleAsync(request));
            _paymentGatewayMock.Verify(pg => pg.ProcessPaymentAsync(It.IsAny<ProcessPaymentRequest>()), Times.Never);
        }





    }
}
