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
            services.AddScoped<ITicketRepository, JsonTicketRepository>();

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SupportDesk",
                "customers.json");

            services.AddScoped<ICustomerRepository, JsonCustomerRepository>(serviceProvider =>
                new JsonCustomerRepository(path));
            services.AddScoped<ICommentRepository, JsonCommentRepository>();
            return services;
        }
    }
}
