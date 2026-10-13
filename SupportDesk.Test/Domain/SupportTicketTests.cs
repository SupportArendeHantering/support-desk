using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Test.Domain;

public class SupportTicketTests
{
    [Fact]
    public void SupportTicket_WhenCreated_HasStatusNew()
    {
        // Arrange & Act
        var ticket = CreateTicket();

        // Assert
        Assert.Equal(TicketStatus.New, ticket.Status);
    }

    [Fact]
    public void ChangeTitle_WithValidTitle_ChangesTitle()
    {
        // Arrange
        var ticket = CreateTicket();

        // Act
        ticket.ChangeTitle("Updated title");

        // Assert
        Assert.Equal("Updated title", ticket.Title.Value);
    }

    [Fact]
    public void ChangeDescription_WithValidDescription_ChangesDescription()
    {
        // Arrange
        var ticket = CreateTicket();

        // Act
        ticket.ChangeDescription("Updated description");

        // Assert
        Assert.Equal("Updated description", ticket.Description.Value);
    }

    [Fact]
    public void ChangePriority_ChangesPriority()
    {
        // Arrange
        var ticket = CreateTicket();

        // Act
        ticket.ChangePriority(TicketPriority.High);

        // Assert
        Assert.Equal(TicketPriority.High, ticket.Priority);
    }

    [Fact]
    public void ChangeStatus_ToOngoingWithoutAssignee_ThrowsInvalidOperationException()
    {
        // Arrange
        var ticket = CreateTicket();

        // Act
        var act = () => ticket.ChangeStatus(TicketStatus.Ongoing);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ChangeStatus_ToResolvedWithoutAssignee_ThrowsInvalidOperationException()
    {
        // Arrange
        var ticket = CreateTicket();

        // Act
        var act = () => ticket.ChangeStatus(TicketStatus.Resolved);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void ChangeStatus_ToOngoingWithAssignee_ChangesStatus()
    {
        // Arrange
        var ticket = CreateTicket();
        ticket.AssignAssignee(1);

        // Act
        ticket.ChangeStatus(TicketStatus.Ongoing);

        // Assert
        Assert.Equal(TicketStatus.Ongoing, ticket.Status);
    }

    [Fact]
    public void ChangeStatus_ToResolvedWithAssignee_ChangesStatus()
    {
        // Arrange
        var ticket = CreateTicket();
        ticket.AssignAssignee(1);

        // Act
        ticket.ChangeStatus(TicketStatus.Resolved);

        // Assert
        Assert.Equal(TicketStatus.Resolved, ticket.Status);
    }

    [Fact]
    public void AssignCustomerId_AssignsCustomer()
    {
        // Arrange
        var ticket = CreateTicket();
        var customerId = Guid.NewGuid();

        // Act
        ticket.AssignCustomerId(customerId);

        // Assert
        Assert.Equal(customerId, ticket.AssignedCustomerId);
    }

    [Fact]
    public void AssignAssignee_AssignsAssignee()
    {
        // Arrange
        var ticket = CreateTicket();

        // Act
        ticket.AssignAssignee(1);

        // Assert
        Assert.Equal(1, ticket.AssigneeId);
    }

    [Fact]
    public void ChangePriority_WithInvalidPriority_ThrowsArgumentException()
    {
        var ticket = CreateTicket();

        var act = () =>
            ticket.ChangePriority((TicketPriority)999);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ChangeStatus_WithInvalidStatus_ThrowsArgumentException()
    {
        var ticket = CreateTicket();

        var act = () =>
            ticket.ChangeStatus((TicketStatus)999);

        Assert.Throws<ArgumentException>(act);
    }

    private static SupportTicket CreateTicket()
    {
        return new SupportTicket(
            new TicketTitle("Test ticket"),
            new TicketDescription("Test description"))
        {
            Id = Guid.NewGuid()
        };
    }
}