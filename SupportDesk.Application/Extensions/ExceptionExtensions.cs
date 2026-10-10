namespace SupportDesk.Application.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToUiMessage(this Exception exception)
        {
            var message = exception.Message;
            var index = message.IndexOf(" (Parameter", StringComparison.Ordinal);
            return index > 0 ? message[..index] : message;
        }
    }