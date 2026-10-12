using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Comments.Interfaces;
using SupportDesk.Application.Customers.Interfaces;
using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Infrastructure.Comments;
using SupportDesk.Infrastructure.Customers;
using SupportDesk.Infrastructure.Tickets;

namespace SupportDesk.Infrastructure
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var customerPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SupportDesk",
                "customers.json");

            var ticketPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SupportDesk",
                "tickets.json");

            services.AddScoped<ITicketRepository, JsonTicketRepository>(serviceProvider =>
                new JsonTicketRepository(ticketPath));

            services.AddScoped<ICustomerRepository, JsonCustomerRepository>(serviceProvider =>
                new JsonCustomerRepository(customerPath));

            services.AddScoped<ICommentRepository, JsonCommentRepository>(serviceProvider =>
                new JsonCommentRepository(ticketPath));

            return services;
        }
    }
}