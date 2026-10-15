using SupportDesk.Application.Comments;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Tickets;
using SupportDesk.Domain.Models;
using SupportDesk.Infrastructure.Comments;
using SupportDesk.Infrastructure.Customers;
using SupportDesk.Infrastructure.Tickets;

namespace SupportDesk.Test.Integration;

public class JsonPersistenceIntegrationTests : IDisposable
{
    private readonly string _directory;
    private readonly string _customerPath;
    private readonly string _ticketPath;

    public JsonPersistenceIntegrationTests()
    {
        _directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_directory);

        _customerPath = Path.Combine(
            _directory,
            "customers.json");

        _ticketPath = Path.Combine(
            _directory,
            "tickets.json");
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, true);
        }
    }

    [Fact]
    public async Task FullWorkflow_PersistsCustomerTicketAssigneeStatusAndComment()
    {
        // Arrange
        var customerRepository =
            new JsonCustomerRepository(_customerPath);

        var ticketRepository =
            new JsonTicketRepository(_ticketPath);

        var commentRepository =
            new JsonCommentRepository(_ticketPath);

        var customerService =
            new CustomerService(customerRepository);

        var ticketService =
            new TicketService(
                ticketRepository,
                customerService);

        var commentService =
            new CommentService(commentRepository);

        // Act
        var customer =
            await customerService.RegisterCustomerAsync(
                "Integration Customer",
                "integration@example.com");

        var ticket =
            await ticketService.RegisterTicketAsync(
                "Integration ticket",
                "Integration description",
                customer.Id,
                TicketPriority.High);

        await ticketService.AssignAssigneeAsync(
            ticket,
            2);

        await ticketService.ChangeStatusAsync(
            ticket,
            TicketStatus.Ongoing);

        await commentService.AddCommentAsync(
            ticket.Id,
            2,
            "Integration comment");

        // New repository instances simulate a new read from disk
        var newCustomerRepository =
            new JsonCustomerRepository(_customerPath);

        var newTicketRepository =
            new JsonTicketRepository(_ticketPath);

        var newCommentRepository =
            new JsonCommentRepository(_ticketPath);

        var loadedCustomers =
            await newCustomerRepository.GetAllCustomersAsync();

        var loadedTickets =
            await newTicketRepository.GetAllTicketsAsync();

        var loadedComments =
            await newCommentRepository
                .GetCommentsByTicketIdAsync(ticket.Id);

        // Assert
        var loadedCustomer =
            Assert.Single(loadedCustomers);

        var loadedTicket =
            Assert.Single(loadedTickets);

        var loadedComment =
            Assert.Single(loadedComments);

        Assert.Equal(
            customer.Id,
            loadedCustomer.Id);

        Assert.Equal(
            ticket.Id,
            loadedTicket.Id);

        Assert.Equal(
            customer.Id,
            loadedTicket.AssignedCustomerId);

        Assert.Equal(
            2,
            loadedTicket.AssigneeId);

        Assert.Equal(
            TicketStatus.Ongoing,
            loadedTicket.Status);

        Assert.Equal(
            TicketPriority.High,
            loadedTicket.Priority);

        Assert.Equal(
            "Integration ticket",
            loadedTicket.Title.Value);

        Assert.Equal(
            "Integration description",
            loadedTicket.Description.Value);

        Assert.Equal(
            ticket.Id,
            loadedComment.TicketId);

        Assert.Equal(
            2,
            loadedComment.CreatedBy);

        Assert.Equal(
            "Integration comment",
            loadedComment.CommentText.Value);
    }

    [Fact]
    public async Task InvalidJson_WhenReadFails_DoesNotOverwriteExistingFile()
    {
        // Arrange
        const string invalidJson =
            "{ this is not valid json";

        await File.WriteAllTextAsync(
            _ticketPath,
            invalidJson);

        var repository =
            new JsonTicketRepository(_ticketPath);

        // Act
        await Assert.ThrowsAnyAsync<Exception>(
            () => repository.GetAllTicketsAsync());

        // Assert
        var fileContent =
            await File.ReadAllTextAsync(_ticketPath);

        Assert.Equal(
            invalidJson,
            fileContent);
    }
}