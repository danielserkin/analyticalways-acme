public record CourseName
{
    public string Value { get; }

    public CourseName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));

        Value = value;
    }
}