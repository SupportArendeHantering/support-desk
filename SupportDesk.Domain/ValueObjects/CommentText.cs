namespace SupportDesk.Domain.ValueObjects;

public record CommentText
{
    public string Value { get; }

    public CommentText (string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Comment text is required", nameof(value));
        value = value.Trim();

        Value = value;
    }
}
