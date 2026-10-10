namespace SupportDesk.Domain.ValueObjects;

public record TicketTitle
{
    public string Value { get; } 

    public TicketTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Du måste ange en titel.",nameof(value));

        value = value.Trim();

        Value = value;
    }
}
