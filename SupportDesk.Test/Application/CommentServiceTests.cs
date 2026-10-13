using NSubstitute;
using SupportDesk.Application.Comments;
using SupportDesk.Application.Comments.Interfaces;
using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;

namespace SupportDesk.Test.Application;

public class CommentServiceTests
{
    [Fact]
    public async Task AddCommentAsync_WithValidInput_AddsCommentToRepository()
    {
        // Arrange
        var repository = Substitute.For<ICommentRepository>();
        var service = new CommentService(repository);

        var ticketId = Guid.NewGuid();
        var assigneeId = 1;

        // Act
        await service.AddCommentAsync(
            ticketId,
            assigneeId,
            "Test comment");

        // Assert
        await repository.Received(1).AddCommentAsync(
            Arg.Is<TicketComment>(comment =>
                comment.CommentId != Guid.Empty &&
                comment.TicketId == ticketId &&
                comment.CreatedBy == assigneeId &&
                comment.CommentText.Value == "Test comment"));
    }


    [Fact]
    public async Task AddCommentAsync_WhenTicketIdIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var repository = Substitute.For<ICommentRepository>();
        var service = new CommentService(repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddCommentAsync(
                Guid.Empty,
                1,
                "Test comment"));
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AddCommentAsync_WhenAssigneeIdIsInvalid_ThrowsArgumentException(
        int assigneeId)
    {
        // Arrange
        var repository = Substitute.For<ICommentRepository>();
        var service = new CommentService(repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddCommentAsync(
                Guid.NewGuid(),
                assigneeId,
                "Test comment"));
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AddCommentAsync_WhenCommentIsEmpty_ThrowsArgumentException(
        string commentText)
    {
        // Arrange
        var repository = Substitute.For<ICommentRepository>();
        var service = new CommentService(repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddCommentAsync(
                Guid.NewGuid(),
                1,
                commentText));
    }


    [Fact]
    public async Task GetCommentsByTicketIdAsync_ReturnsCommentsFromRepository()
    {
        // Arrange
        var repository = Substitute.For<ICommentRepository>();
        var service = new CommentService(repository);

        var ticketId = Guid.NewGuid();

        IReadOnlyList<TicketComment> comments =
        [
            new TicketComment
            {
                CommentId = Guid.NewGuid(),
                TicketId = ticketId,
                CreatedBy = 1,
                CommentText = new CommentText("First comment"),
                CreatedAt = DateTime.UtcNow
            },

            new TicketComment
            {
                CommentId = Guid.NewGuid(),
                TicketId = ticketId,
                CreatedBy = 2,
                CommentText = new CommentText("Second comment"),
                CreatedAt = DateTime.UtcNow
            }
        ];

        repository
            .GetCommentsByTicketIdAsync(ticketId)
            .Returns(comments);

        // Act
        var result =
            await service.GetCommentsByTicketIdAsync(ticketId);

        // Assert
        Assert.Same(comments, result);
    }


    [Fact]
    public async Task GetCommentsByTicketIdAsync_WhenTicketIdIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var repository = Substitute.For<ICommentRepository>();
        var service = new CommentService(repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetCommentsByTicketIdAsync(Guid.Empty));
    }
}