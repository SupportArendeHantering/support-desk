using System.Net.Mail;

namespace SupportDesk.Domain.ValueObjects;

public record Email
{
    public string Value { get; }
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.", nameof(value));

        var trimmedEmail = value.Trim();

        if (!MailAddress.TryCreate(trimmedEmail, out var emailAddress) || emailAddress.Address != trimmedEmail)
            throw new ArgumentException("Invalid email format.", nameof(value));

        Value = trimmedEmail;
    }

    public override string ToString() => Value;
}
