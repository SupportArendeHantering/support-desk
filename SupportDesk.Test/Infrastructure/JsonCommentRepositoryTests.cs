using SupportDesk.Domain.Models;
using SupportDesk.Domain.ValueObjects;
using SupportDesk.Infrastructure.Comments;
using SupportDesk.Infrastructure.Tickets.Models;
using System.Text.Json;

namespace SupportDesk.Test.Infrastructure;

public class JsonCommentRepositoryTests : IDisposable
{
    private readonly string _directory;
    private readonly string _filePath;
    private readonly JsonCommentRepository _repository;

    public JsonCommentRepositoryTests()
    {
        _directory = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_directory);

        _filePath = Path.Combine(_directory, "tickets.json");

        _repository = new JsonCommentRepository(_filePath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, true);
    }

    [Fact]
    public async Task GetCommentsByTicketIdAsync_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        // Act
        var result =
            await _repository.GetCommentsByTicketIdAsync(Guid.NewGuid());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCommentsByTicketIdAsync_WhenTicketDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        var existingTicketId = Guid.NewGuid();

        await WriteTicketsAsync(
            new TicketData
            {
                Id = existingTicketId
            });

        // Act
        var result =
            await _repository.GetCommentsByTicketIdAsync(Guid.NewGuid());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddCommentAsync_WhenFileDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var comment = CreateComment(Guid.NewGuid());

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.AddCommentAsync(comment));

        // Assert
        Assert.Equal(
            "Ticket does not exist.",
            exception.Message);
    }

    [Fact]
    public async Task AddCommentAsync_WhenTicketDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        await WriteTicketsAsync(
            new TicketData
            {
                Id = Guid.NewGuid()
            });

        var comment = CreateComment(Guid.NewGuid());

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.AddCommentAsync(comment));

        // Assert
        Assert.Equal(
            "Ticket not found.",
            exception.Message);
    }

    [Fact]
    public async Task AddCommentAsync_WithValidComment_AddsCommentToTicket()
    {
        // Arrange
        var ticketId = Guid.NewGuid();

        await WriteTicketsAsync(
            new TicketData
            {
                Id = ticketId
            });

        var comment = CreateComment(ticketId);

        // Act
        await _repository.AddCommentAsync(comment);

        var result =
            await _repository.GetCommentsByTicketIdAsync(ticketId);

        // Assert
        var addedComment = Assert.Single(result);

        Assert.Equal(comment.CommentId, addedComment.CommentId);
        Assert.Equal(comment.TicketId, addedComment.TicketId);
        Assert.Equal(comment.CreatedBy, addedComment.CreatedBy);
        Assert.Equal(
            comment.CommentText.Value,
            addedComment.CommentText.Value);
        Assert.Equal(comment.CreatedAt, addedComment.CreatedAt);
    }

    [Fact]
    public async Task AddCommentAsync_MultipleTimes_PreservesAllComments()
    {
        // Arrange
        var ticketId = Guid.NewGuid();

        await WriteTicketsAsync(
            new TicketData
            {
                Id = ticketId
            });

        var firstComment =
            CreateComment(ticketId, "First comment");

        var secondComment =
            CreateComment(ticketId, "Second comment");

        // Act
        await _repository.AddCommentAsync(firstComment);
        await _repository.AddCommentAsync(secondComment);

        var result =
            await _repository.GetCommentsByTicketIdAsync(ticketId);

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            comment => comment.CommentId == firstComment.CommentId);

        Assert.Contains(
            result,
            comment => comment.CommentId == secondComment.CommentId);
    }

    [Fact]
    public async Task AddCommentAsync_DoesNotAddCommentToOtherTicket()
    {
        // Arrange
        var firstTicketId = Guid.NewGuid();
        var secondTicketId = Guid.NewGuid();

        await WriteTicketsAsync(
            new TicketData
            {
                Id = firstTicketId
            },
            new TicketData
            {
                Id = secondTicketId
            });

        var comment = CreateComment(firstTicketId);

        // Act
        await _repository.AddCommentAsync(comment);

        var firstTicketComments =
            await _repository.GetCommentsByTicketIdAsync(firstTicketId);

        var secondTicketComments =
            await _repository.GetCommentsByTicketIdAsync(secondTicketId);

        // Assert
        Assert.Single(firstTicketComments);
        Assert.Empty(secondTicketComments);
    }

    private async Task WriteTicketsAsync(params TicketData[] tickets)
    {
        var json = JsonSerializer.Serialize(tickets);

        await File.WriteAllTextAsync(
            _filePath,
            json);
    }

    private static TicketComment CreateComment(
        Guid ticketId,
        string text = "Test comment")
    {
        return new TicketComment
        {
            CommentId = Guid.NewGuid(),
            TicketId = ticketId,
            CreatedBy = 1,
            CommentText = new CommentText(text),
            CreatedAt = new DateTime(
                2026,
                10,
                12,
                12,
                0,
                0,
                DateTimeKind.Utc)
        };
    }
}