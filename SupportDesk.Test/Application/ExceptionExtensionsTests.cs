using SupportDesk.Application.Extensions;

namespace SupportDesk.Application.Tests.Extensions;

public class ExceptionExtensionsTests
{
    [Fact]
    public void ToUiMessage_PlainMessage_ReturnsMessageUnchanged()
    {
        var exception = new InvalidOperationException("Something went wrong");

        Assert.Equal("Something went wrong", exception.ToUiMessage());
    }

    [Fact]
    public void ToUiMessage_MessageWithParameterSuffix_StripsSuffix()
    {
        var exception = new ArgumentException("Title is required", "title");

        Assert.Equal("Title is required", exception.ToUiMessage());
    }

    [Fact]
    public void ToUiMessage_ExplicitParameterSuffix_StripsSuffix()
    {
        var exception = new Exception("Invalid value (Parameter 'x')");

        Assert.Equal("Invalid value", exception.ToUiMessage());
    }

    [Fact]
    public void ToUiMessage_MessageStartsWithParameterMarker_ReturnsOriginal()
    {
        // index == 0, so the message is not trimmed
        var exception = new Exception(" (Parameter 'x')");

        Assert.Equal(" (Parameter 'x')", exception.ToUiMessage());
    }

    [Fact]
    public void ToUiMessage_EmptyMessage_ReturnsDefaultExceptionMessage()
    {
        var exception = new Exception(string.Empty);

        Assert.Equal(string.Empty, exception.ToUiMessage());
    }

    [Fact]
    public void ToUiMessage_MultipleParameterMarkers_CutsAtFirst()
    {
        var exception = new Exception("A (Parameter 'a') B (Parameter 'b')");

        Assert.Equal("A", exception.ToUiMessage());
    }

    [Fact]
    public void ToUiMessage_MarkerIsCaseSensitive()
    {
        var exception = new Exception("Oops (parameter 'x')");

        Assert.Equal("Oops (parameter 'x')", exception.ToUiMessage());
    }
}
