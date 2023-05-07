using System.Text.Json;
using System.Text.Json.Serialization;

namespace MPay.API.IntegrationTests;

public static class Extensions
{
    private static JsonSerializerOptions DefaultJsonSerializerOptions =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

    public static async Task<T?> GetResultAsync<T>(this HttpResponseMessage response) where T : class
    {
        var content = await response.Content.ReadAsStringAsync();

        if (typeof(T) == typeof(string)) return content as T;

        return JsonSerializer.Deserialize<T>(content, DefaultJsonSerializerOptions);
    }
}