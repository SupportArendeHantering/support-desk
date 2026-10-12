using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Comments;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Notifications;
using SupportDesk.Application.Tickets;

namespace SupportDesk.Application
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<ITicketService, TicketService>();
            services.AddTransient<ITicketAssigneeService, TicketAssigneeService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddSingleton<INotificationService, NotificationService>();
            return services;
        }
    }
}
