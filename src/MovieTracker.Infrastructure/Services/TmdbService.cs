using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Infrastructure.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.themoviedb.org/3";
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";

    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Tmdb:ApiKey"]!;
    }

    public async Task<List<TmdbMovieDto>> SearchMoviesAsync(string query, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<TmdbSearchResponse<TmdbMovieDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Results ?? new List<TmdbMovieDto>();
    }

    public async Task<TmdbMovieDetailsDto?> GetMovieDetailsAsync(int tmdbId, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/movie/{tmdbId}?api_key={_apiKey}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var movie = JsonSerializer.Deserialize<TmdbMovieDetailsDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return movie;
    }

    public async Task<List<TmdbSeriesDto>> SearchSeriesAsync(string query, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/search/tv?api_key={_apiKey}&query={Uri.EscapeDataString(query)}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<TmdbSearchResponse<TmdbSeriesDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result?.Results ?? new List<TmdbSeriesDto>();
    }

    public async Task<TmdbSeriesDetailsDto?> GetSeriesDetailsAsync(int tmdbId, CancellationToken cancellationToken = default)
    {
        var url = $"{BaseUrl}/tv/{tmdbId}?api_key={_apiKey}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var series = JsonSerializer.Deserialize<TmdbSeriesDetailsDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Fetch episodes for each season
        if (series != null && series.Seasons.Any())
        {
            foreach (var season in series.Seasons)
            {
                var episodesUrl = $"{BaseUrl}/tv/{tmdbId}/season/{season.SeasonNumber}?api_key={_apiKey}";
                var episodesResponse = await _httpClient.GetAsync(episodesUrl, cancellationToken);
                
                if (episodesResponse.IsSuccessStatusCode)
                {
                    var episodesJson = await episodesResponse.Content.ReadAsStringAsync(cancellationToken);
                    var seasonDetails = JsonSerializer.Deserialize<SeasonDetailsResponse>(episodesJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (seasonDetails?.Episodes != null)
                    {
                        season.Episodes = seasonDetails.Episodes;
                    }
                }
            }
        }

        return series;
    }

    private class TmdbSearchResponse<T>
    {
        public List<T> Results { get; set; } = new();
    }

    private class SeasonDetailsResponse
    {
        public List<EpisodeDto> Episodes { get; set; } = new();
    }
}