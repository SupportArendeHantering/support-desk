namespace SupportDesk.Domain.ValueObjects;

public record CommentText
{
    public string Value { get; }

    public CommentText (string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Du måste ange en kommentar", nameof(value));
        value = value.Trim();

        Value = value;
    }
}
