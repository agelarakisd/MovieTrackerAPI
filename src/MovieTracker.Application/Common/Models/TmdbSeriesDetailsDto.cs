using System.Text.Json.Serialization;

namespace MovieTracker.Application.Common.Models;

public class TmdbSeriesDetailsDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;
    
    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }
    
    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
    
    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; }
    
    [JsonPropertyName("number_of_seasons")]
    public int NumberOfSeasons { get; set; }
    
    [JsonPropertyName("number_of_episodes")]
    public int NumberOfEpisodes { get; set; }
    
    [JsonPropertyName("genres")]
    public List<GenreDto> Genres { get; set; } = new();
    
    [JsonPropertyName("seasons")]
    public List<SeasonDto> Seasons { get; set; } = new();
}