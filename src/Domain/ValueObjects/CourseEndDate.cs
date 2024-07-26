public record CourseEndDate
{
    public DateTime Value { get; }

    public CourseEndDate(DateTime value)
    {
        Value = value;
    }
}