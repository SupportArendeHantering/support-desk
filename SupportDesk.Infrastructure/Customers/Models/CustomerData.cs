namespace SupportDesk.Infrastructure.Customers.Models;

public class CustomerData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}