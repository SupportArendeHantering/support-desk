using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Domain.Models;

public class Customer(CustomerName customerName, Email email)
{
    public Guid Id { get; set; }
    public CustomerName Name { get; set; } = customerName;
    public Email Email { get; set; } = email;

    public void Rename(CustomerName newName)
    {
        Name = newName;
    }

    public void ChangeEmail(Email newEmail)
    {
        Email = newEmail;
    }
}
