using SupportDesk.Domain.Models;

namespace SupportDesk.Application.Customers
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(Guid customerId);
        Task<Customer> RegisterCustomerAsync(string name, string email);

        Task<IReadOnlyList<Customer>> GetAllCustomersAsync();

        Task<Customer> UpdateCustomerAsync(
            Customer customer,
            string name,
            string email);
    }
}