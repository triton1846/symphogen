using System.Text.Json;

namespace BlazorWebAppSymphogen.Extensions;

/// <summary>
/// Extension methods for deep-cloning objects using JSON serialization
/// </summary>
public static class JsonCloningExtensions
{
    /// <summary>
    /// Creates a deep copy of an object using JSON serialization/deserialization.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the object to</typeparam>
    /// <param name="source">The source object to copy</param>
    /// <param name="options">Optional JSON serializer options</param>
    /// <returns>A deep copy of the source object</returns>
    public static T DeepCopy<T>(this T source, JsonSerializerOptions? options = null) where T : class
    {
        ArgumentNullException.ThrowIfNull(source);

        var json = JsonSerializer.Serialize(source, options);
        var copy = JsonSerializer.Deserialize<T>(json, options);
        return copy ?? throw new InvalidOperationException($"Failed to create a copy of {typeof(T).Name}");
    }
}
