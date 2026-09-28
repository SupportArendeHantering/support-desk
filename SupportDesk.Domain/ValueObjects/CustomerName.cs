namespace SupportDesk.Domain.ValueObjects;

public record CustomerName
{
    public string Value { get; }

    public CustomerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be empty.", nameof(value));

        var trimmedName = value.Trim();

        if (trimmedName.Length < 2 || trimmedName.Length > 30)
            throw new ArgumentException("Name must be between 2 and 30 characters.", nameof(value));

        Value = trimmedName;
    }

    public override string ToString() => Value;
}
