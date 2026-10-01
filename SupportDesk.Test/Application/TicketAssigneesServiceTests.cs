using SupportDesk.Application.Tickets;

namespace SupportDesk.Test.Tickets;

public class TicketAssigneesServiceTests
{
    private readonly TicketAssigneesService _ticketAssigneeService = new();

    [Fact]
    public void GetAllAssignees_ReturnsAllPredefinedAssignees()
    {
        // Act
        var result = _ticketAssigneeService.GetAllAssignees();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);

        Assert.Collection(result,
            assignee =>
            {
                Assert.Equal(1, assignee.Id);
                Assert.Equal("Alice Johnson", assignee.Name);
            },
            assignee =>
            {
                Assert.Equal(2, assignee.Id);
                Assert.Equal("Bob Smith", assignee.Name);
            },
            assignee =>
            {
                Assert.Equal(3, assignee.Id);
                Assert.Equal("Charlie Brown", assignee.Name);
            },
            assignee =>
            {
                Assert.Equal(4, assignee.Id);
                Assert.Equal("David Wilson", assignee.Name);
            },
            assignee =>
            {
                Assert.Equal(5, assignee.Id);
                Assert.Equal("Eva Davis", assignee.Name);
            });
    }

    [Theory]
    [InlineData(1, "Alice Johnson")]
    [InlineData(3, "Charlie Brown")]
    [InlineData(5, "Eva Davis")]
    public void GetAssigneeById_WhenIdExists_ReturnsMatchingAssignee(int id, string expectedName)
    {
        // Act
        var result = _ticketAssigneeService.GetAssigneeById(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.Equal(expectedName, result.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(999)]
    public void GetAssigneeById_WhenIdDoesNotExist_ReturnsNull(int id)
    {
        // Act
        var result = _ticketAssigneeService.GetAssigneeById(id);

        // Assert
        Assert.Null(result);
    }
}
