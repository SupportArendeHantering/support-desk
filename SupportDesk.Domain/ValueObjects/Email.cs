using System.Net.Mail;

namespace SupportDesk.Domain.ValueObjects;

public record Email
{
    public string Value { get; }
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("E-post kan inte vara tomt", nameof(value));

        var trimmedEmail = value.Trim();

        if (!MailAddress.TryCreate(trimmedEmail, out var emailAddress) || emailAddress.Address != trimmedEmail)
            throw new ArgumentException("Ogiltigt e-post format.", nameof(value));

        Value = trimmedEmail.ToLower();
    }

    public override string ToString() => Value;
}
