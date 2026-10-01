using NSubstitute;
using SupportDesk.Application.Customers;
using SupportDesk.Application.Tickets;
using SupportDesk.Application.Tickets.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Test.Application;

public class TicketServiceTests
{
    [Fact]
    public async Task RegisterTicketAsync_WithEmptyCustomerId_ThrowsArgumentException()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();

        // Act
        var act = () => ticketService.RegisterTicketAsync("Title", "Description", Guid.Empty, TicketPriority.Normal);

        // Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        Assert.Equal("customerId", exception.ParamName);

        await ticketRepository.DidNotReceive().AddTicketAsync(Arg.Any<SupportTicket>());
    }

    [Fact]
    public async Task RegisterTicketAsync_WithValidInput_AddsTicketAndReturnsIt()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var customerId = Guid.NewGuid();

        // Act
        var result = await ticketService.RegisterTicketAsync("  Printer issue  ", "  Cannot print  ", customerId, TicketPriority.High);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Printer issue", result.Title.Value);
        Assert.Equal("Cannot print", result.Description.Value);
        Assert.Equal(customerId, result.AssignedCustomerId);
        Assert.Equal(TicketPriority.High, result.Priority);

        await ticketRepository.Received(1).AddTicketAsync(result);
    }

    [Fact]
    public async Task GetAllTicketsAsync_ReturnsTicketsSortedByCreatedAtDescending()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();

        var oldTicket = CreateTicket(createdAt: DateTime.UtcNow.AddDays(-2));
        var midTicket = CreateTicket(createdAt: DateTime.UtcNow.AddDays(-1));
        var newTicket = CreateTicket(createdAt: DateTime.UtcNow);

        // Act
        ticketRepository.GetAllTicketsAsync().Returns([midTicket, oldTicket, newTicket]);

        var result = await ticketService.GetAllTicketsAsync();

        // Assert
        Assert.Equal([newTicket, midTicket, oldTicket], result);
        await ticketRepository.Received(1).GetAllTicketsAsync();
    }

    [Fact]
    public async Task UpdateTicketDetailsAsync_ChangesTicketAndUpdatesRepository()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var ticket = CreateTicket();
        
        // Act
        await ticketService.UpdateTicketDetailsAsync(ticket, "  New title  ", "  New description  ");

        // Assert
        Assert.Equal("New title", ticket.Title.Value);
        Assert.Equal("New description", ticket.Description.Value);
        await ticketRepository.Received(1).UpdateTicketAsync(ticket);
    }

    [Fact]
    public async Task ChangePriorityAsync_ChangesPriorityAndUpdatesRepository()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var ticket = CreateTicket();
        
        // Act
        await ticketService.ChangePriorityAsync(ticket, TicketPriority.High);

        // Assert
        Assert.Equal(TicketPriority.High, ticket.Priority);
        await ticketRepository.Received(1).UpdateTicketAsync(ticket);
    }

    [Fact]
    public async Task ChangeStatusAsync_WithAssignee_ChangesStatusAndUpdatesRepository()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var ticket = CreateTicket();
        ticket.AssignAssignee(1);

        // Act
        await ticketService.ChangeStatusAsync(ticket, TicketStatus.Ongoing);

        // Assert
        Assert.Equal(TicketStatus.Ongoing, ticket.Status);
        await ticketRepository.Received(1).UpdateTicketAsync(ticket);
    }

    [Fact]
    public async Task ChangeStatusAsync_ToOngoingWithoutAssignee_ThrowsAndDoesNotUpdateRepository()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var ticket = CreateTicket();

        // Act
        var act = () => ticketService.ChangeStatusAsync(ticket, TicketStatus.Ongoing);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
        await ticketRepository.DidNotReceive().UpdateTicketAsync(Arg.Any<SupportTicket>());
    }

    [Fact]
    public async Task AssignCustomerToTicketAsync_AssignsCustomerAndUpdatesRepository()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var ticket = CreateTicket();
        var firstId = ticket.AssignedCustomerId;
        var newCustomerId = Guid.NewGuid();

        // Act
        await ticketService.AssignCustomerToTicketAsync(ticket, newCustomerId);

        // Assert
        Assert.NotEqual(firstId, ticket.AssignedCustomerId);
        Assert.Equal(newCustomerId, ticket.AssignedCustomerId);
        await ticketRepository.Received(1).UpdateTicketAsync(ticket);
    }

    [Fact]
    public async Task GetAssignedCustomerAsync_ReturnsCustomerFromCustomerService()
    {
        // Arrange
        var ticketRepository = Substitute.For<ITicketRepository>();
        var customerService = Substitute.For<ICustomerService>();
        var ticketService = new TicketService(ticketRepository, customerService);

        var customerId = Guid.NewGuid();
        var expectedCustomer = new Customer(new CustomerName("Morty"), new Email("rick_sanche@example.com"))
        {
            Id = customerId
        };

        customerService.GetCustomerByIdAsync(customerId).Returns(expectedCustomer);

        // Act
        var ticket = CreateTicket();
        ticket.AssignCustomerId(customerId);

        var result = await ticketService.GetAssignedCustomerAsync(ticket);
        
        // Assert
        Assert.Same(expectedCustomer, result);
        await customerService.Received(1).GetCustomerByIdAsync(customerId);
    }

    [Fact]
    public async Task AssignAssigneeAsync_AssignsAssigneeAndUpdatesRepository()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();
        var ticket = CreateTicket();
        
        // Act
        await ticketService.AssignAssigneeAsync(ticket, 7);

        // Assert
        Assert.Equal(7, ticket.AssigneeId);
        await ticketRepository.Received(1).UpdateTicketAsync(ticket);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new TicketStatus[0])]
    public async Task GetTicketsByStatusAsync_WithEmptyStatuses_ReturnsAllTickets(TicketStatus[]? statuses)
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();

        IReadOnlyList<SupportTicket> tickets = [CreateTicket(), CreateTicket()];
        ticketRepository.GetAllTicketsAsync().Returns(tickets);

        // Act
        var result = await ticketService.GetTicketsByStatusAsync(statuses);

        // Assert
        Assert.Same(tickets, result);
    }

    [Fact]
    public async Task GetTicketsByStatusAsync_WithStatuses_FiltersTickets()
    {
        // Arrange
        var (ticketService, ticketRepository) = CreateService();

        var newTicket = CreateTicket();

        var ongoingTicket = CreateTicket();
        ongoingTicket.AssignAssignee(1);
        ongoingTicket.ChangeStatus(TicketStatus.Ongoing);

        var resolvedTicket = CreateTicket();
        resolvedTicket.AssignAssignee(2);
        resolvedTicket.ChangeStatus(TicketStatus.Resolved);

        ticketRepository.GetAllTicketsAsync().Returns([newTicket, ongoingTicket, resolvedTicket]);

        // Act
        var result = await ticketService.GetTicketsByStatusAsync([TicketStatus.New, TicketStatus.Resolved]);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(newTicket, result);
        Assert.Contains(resolvedTicket, result);
        Assert.DoesNotContain(ongoingTicket, result);
    }

    private (TicketService ticketService, ITicketRepository ticketRepository) CreateService()
    {
        var ticketRepository = Substitute.For<ITicketRepository>();
        var customerService = Substitute.For<ICustomerService>();
        var ticketService = new TicketService(ticketRepository, customerService);

        return (ticketService, ticketRepository);
    }

    private static SupportTicket CreateTicket(DateTime? createdAt = null)
    {
        return new SupportTicket(new TicketTitle("Title"), new TicketDescription("Description"))
        {
            Id = Guid.NewGuid(),
            AssignedCustomerId = Guid.NewGuid(),
            CreatedAt = createdAt ?? DateTime.UtcNow,
            Priority = TicketPriority.Low
        };
    }
}