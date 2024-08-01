using System.Text.Json;

namespace Toggl2Vertec;

public static class JsonElementExtensions
{
    public static string GetStringSafe(this JsonElement element)
    {
            return element.ValueKind == JsonValueKind.String ? element.GetString() : null;
        }

    public static bool HasValue(this JsonElement element)
    {
            return !(element.ValueKind == JsonValueKind.Undefined || element.ValueKind == JsonValueKind.Null);
        }

    public static JsonElement Get(this JsonElement start, string path)
    {
            var segments = path.Split(".");
            var current = start;
            foreach (var segment in segments)
            {
                current.TryGetProperty(segment, out current);
                if (current.ValueKind == JsonValueKind.Undefined || current.ValueKind == JsonValueKind.Null)
                {
                    return new JsonElement();
                }
            }

            return current;
        }
}