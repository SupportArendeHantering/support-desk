using System.Text.Json;

namespace SupportDesk.Infrastructure.Settings
{
    internal static class JsonSettings
    {
        internal static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };
    }
}
