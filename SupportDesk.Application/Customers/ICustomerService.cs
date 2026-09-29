using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Customers
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(Guid customerId);
        Task<Customer> RegisterCustomerAsync(string name, string email);
    }
}