namespace SupportDesk.Domain.ValueObjects;

public record TicketDescription
{
    public string Value { get; }

    public TicketDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Description is required",nameof(value));
        value = value.Trim();

        Value = value;
    }
}
