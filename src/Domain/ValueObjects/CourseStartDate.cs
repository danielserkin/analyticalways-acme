public record CourseStartDate
{
    public DateTime Value { get; }

    public CourseStartDate(DateTime value)
    {
        Value = value;
    }
}