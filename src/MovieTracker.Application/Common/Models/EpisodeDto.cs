using System.Text.Json.Serialization;

namespace MovieTracker.Application.Common.Models;

public class EpisodeDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }
    
    [JsonPropertyName("episode_number")]
    public int EpisodeNumber { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;
    
    [JsonPropertyName("air_date")]
    public string? AirDate { get; set; }
}