namespace SupportDesk.Domain.ValueObjects;

public record TicketTitle
{
    public string Value { get; } 

    public TicketTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title is required.",nameof(value));

        value = value.Trim();

        Value = value;
    }
}
