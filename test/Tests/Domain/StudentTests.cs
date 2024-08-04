using AutoFixture;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;

namespace UnitTests.Domain.Entities;

public class StudentTests
{
    private readonly Fixture _fixture;

    public StudentTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldCreateStudent()
    {
        // Arrange
        var name = _fixture.Create<string>();
        var birthDate = GenerateValidBirthDate();

        // Act
        var student = new Student(name, birthDate);

        // Assert
        student.Id.Should().NotBeEmpty();
        student.Name.Should().Be(name);
        student.BirthDate.Should().Be(birthDate);
    }

    [Fact]
    public void Constructor_WithInvalidBirthDate_ShouldThrowInvalidAgeException()
    {
        // Arrange
        var name = _fixture.Create<string>();
        var birthDate = DateTime.Now; 

        // Act & Assert
        Assert.Throws<InvalidAgeException>(() => new Student(name, birthDate));
    }

    [Fact]
    public void Constructor_WithNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        string name = null;
        var birthDate = GenerateValidBirthDate();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Student(name, birthDate));
    }

    private DateTime GenerateValidBirthDate()
    {
        return DateTime.Today.AddYears(-20);
    }
}
