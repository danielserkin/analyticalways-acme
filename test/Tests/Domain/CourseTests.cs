using AutoFixture;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;


namespace UnitTests.Domain.Entities
{
    public class CourseTests
    {
        private readonly Fixture _fixture;

        public CourseTests()
        {
            _fixture = new Fixture();

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
            _fixture.Customize<Student>(composer => composer 
            .FromFactory(() => new Student("John Perez", GenerateValidBirthDate())));
        }
        private DateTime GenerateValidBirthDate()
        {
            return DateTime.Today.AddYears(-20);
        }

        [Fact]
        public void Constructor_WithValidData_ShouldCreateCourse()
        {
            // Arrange
            var name = _fixture.Create<CourseName>();
            var fee = _fixture.Create<CourseFee>();
            var startDate = new CourseStartDate(DateTime.UtcNow.AddDays(1));
            var endDate = new CourseEndDate(DateTime.UtcNow.AddDays(30));

            // Act
            var course = new Course(name, fee, startDate, endDate);

            // Assert
            course.Id.Should().NotBeEmpty(); 
            course.Name.Should().Be(name);
            course.Fee.Should().Be(fee);
            course.StartDate.Should().Be(startDate);
            course.EndDate.Should().Be(endDate);
            course.Students.Should().BeEmpty(); 
        }

        [Fact]
        public void Constructor_WithInvalidDateRange_ShouldThrowInvalidCourseDateRangeException()
        {
            // Arrange
            var name = _fixture.Create<CourseName>();
            var fee = _fixture.Create<CourseFee>();
            var startDate = new CourseStartDate(DateTime.UtcNow.AddDays(30)); 
            var endDate = new CourseEndDate(DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidCourseDateRangeException>(() => new Course(name, fee, startDate, endDate));
        }

        [Fact]
        public void EnrollStudent_WithNewStudent_ShouldAddStudentToList()
        {
            // Arrange
            var course = _fixture.Create<Course>();
            var student = _fixture.Create<Student>();

            // Act
            course.EnrollStudent(student);

            // Assert
            course.Students.Should().ContainSingle(s => s.Id == student.Id);
        }

        [Fact]
        public void EnrollStudent_WithExistingStudent_ShouldThrowStudentAlreadyEnrolledException()
        {
            // Arrange
            var course = _fixture.Create<Course>();
            var student = _fixture.Create<Student>();
            course.EnrollStudent(student);

            // Act & Assert
            Assert.Throws<StudentAlreadyEnrolledException>(() => course.EnrollStudent(student));
        }
    }
}
