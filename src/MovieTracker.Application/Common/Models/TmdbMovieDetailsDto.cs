using System.Text.Json.Serialization;

namespace MovieTracker.Application.Common.Models;

public class TmdbMovieDetailsDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("overview")]
    public string Overview { get; set; } = string.Empty;
    
    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }
    
    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; }
    
    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }
    
    [JsonPropertyName("runtime")]
    public int? Runtime { get; set; }
    
    [JsonPropertyName("genres")]
    public List<GenreDto> Genres { get; set; } = new();
    
    public int? Year => !string.IsNullOrEmpty(ReleaseDate) && ReleaseDate.Length >= 4 
        ? int.Parse(ReleaseDate.Substring(0, 4)) 
        : null;
}