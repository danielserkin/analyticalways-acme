public record CourseName
{
    public string Value { get; }

    public CourseName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El nombre del curso no puede estar vacío.");
        }

        Value = value;
    }
}