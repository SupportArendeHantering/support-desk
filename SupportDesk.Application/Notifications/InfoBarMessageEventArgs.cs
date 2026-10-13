namespace SupportDesk.Application.Notifications;

public class InfoBarMessageEventArgs(string message, InfoType type) : EventArgs
{
    public string Message { get; } = message;
    public InfoType Type { get; } = type;
}
