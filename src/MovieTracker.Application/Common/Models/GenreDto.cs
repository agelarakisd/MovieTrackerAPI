using System.Text.Json.Serialization;

namespace MovieTracker.Application.Common.Models;

public class GenreDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}