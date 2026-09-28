using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Customers.Interfaces;

public interface ICustomerRepository
{
    void AddCustomer(Customer customer);    
}
