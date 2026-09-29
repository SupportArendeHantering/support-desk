using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Customers.Interfaces;

public interface ICustomerRepository
{
    Task AddCustomerAsync(Customer customer);
}