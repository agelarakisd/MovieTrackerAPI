using System.Text.Json.Serialization;

namespace MovieTracker.Application.Common.Models;

public class SeasonDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("episode_count")]
    public int EpisodeCount { get; set; }
    
    public List<EpisodeDto> Episodes { get; set; } = new();
}