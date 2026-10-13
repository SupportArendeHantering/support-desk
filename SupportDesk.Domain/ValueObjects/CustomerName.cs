namespace SupportDesk.Domain.ValueObjects;

public record CustomerName
{
    public string Value { get; }

    public CustomerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Namnet kan inte vara tomt", nameof(value));

        var trimmedName = value.Trim();

        if (trimmedName.Length < 2 || trimmedName.Length > 30)
            throw new ArgumentException("Namnet måste vara längre än 2 tecken och mindre än 30.", nameof(value));

        Value = trimmedName;
    }

    public override string ToString() => Value;
}
