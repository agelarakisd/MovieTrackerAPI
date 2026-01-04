using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using MovieTracker.Domain.Entities;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Commands.AddSeries;

public class AddSeriesCommandHandler : IRequestHandler<AddSeriesCommand, ApiResponse<SeriesResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITmdbService _tmdbService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddSeriesCommandHandler(
        IApplicationDbContext context,
        ITmdbService tmdbService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _tmdbService = tmdbService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<SeriesResponseDto>> Handle(AddSeriesCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<SeriesResponseDto>.FailureResult("User not authenticated");
        }

        // Check if series already exists in user's list (including soft deleted)
        var existingSeries = await _context.Series
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TmdbId == request.TmdbId, cancellationToken);

        if (existingSeries != null)
        {
            // If soft deleted, restore it
            if (existingSeries.IsDeleted)
            {
                existingSeries.IsDeleted = false;
                existingSeries.DeletedAt = null;
                await _context.SaveChangesAsync(cancellationToken);

                var restoredEpisodeCount = await _context.Episodes.CountAsync(e => e.SeriesId == existingSeries.Id, cancellationToken);
                return ApiResponse<SeriesResponseDto>.SuccessResult(
                    MapToDtoWithCount(existingSeries, restoredEpisodeCount), 
                    $"Series restored to your list with {restoredEpisodeCount} episodes");
            }

            return ApiResponse<SeriesResponseDto>.FailureResult("Series already in your list");
        }

        var seriesDetails = await _tmdbService.GetSeriesDetailsAsync(request.TmdbId, cancellationToken);
        if (seriesDetails == null)
        {
            return ApiResponse<SeriesResponseDto>.FailureResult("Series not found in TMDB");
        }

        Console.WriteLine($"TMDB returned {seriesDetails.Seasons.Count} seasons");
        foreach (var season in seriesDetails.Seasons)
        {
            Console.WriteLine($"Season {season.SeasonNumber} has {season.Episodes.Count} episodes");
        }

        var episodes = new List<Episode>();
        foreach (var season in seriesDetails.Seasons.Where(s => s.SeasonNumber > 0)) // Skip season 0 (specials)
        {
            foreach (var episodeDto in season.Episodes)
            {
                var episode = new Episode
                {
                    Id = Guid.NewGuid(),
                    SeriesId = Guid.Empty, 
                    TmdbEpisodeId = episodeDto.Id,
                    SeasonNumber = episodeDto.SeasonNumber,
                    EpisodeNumber = episodeDto.EpisodeNumber,
                    Title = episodeDto.Name,
                    Overview = episodeDto.Overview,
                    AirDate = !string.IsNullOrEmpty(episodeDto.AirDate) ? DateTime.Parse(episodeDto.AirDate) : null,
                    IsWatched = false
                };
                
                episodes.Add(episode);
            }
        }

        var series = new Domain.Entities.Series
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TmdbId = request.TmdbId,
            Title = seriesDetails.Name,
            Overview = seriesDetails.Overview,
            PosterUrl = seriesDetails.PosterPath != null ? $"https://image.tmdb.org/t/p/w500{seriesDetails.PosterPath}" : null,
            BackdropUrl = seriesDetails.BackdropPath != null ? $"https://image.tmdb.org/t/p/w500{seriesDetails.BackdropPath}" : null,
            FirstAirDate = !string.IsNullOrEmpty(seriesDetails.FirstAirDate) ? DateTime.Parse(seriesDetails.FirstAirDate) : null,
            Genre = seriesDetails.Genres.Any() ? string.Join(", ", seriesDetails.Genres.Select(g => g.Name)) : null,
            NumberOfSeasons = seriesDetails.Seasons.Count(s => s.SeasonNumber > 0),
            NumberOfEpisodes = episodes.Count,  
            AddedAt = DateTime.UtcNow
        };

        _context.Series.Add(series);

        foreach (var episode in episodes)
        {
            episode.SeriesId = series.Id;
        }

        _context.Episodes.AddRange(episodes);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<SeriesResponseDto>.SuccessResult(
            MapToDtoWithCount(series, episodes.Count), 
            $"Series added with {episodes.Count} episodes");
    }

    private static SeriesResponseDto MapToDtoWithCount(Domain.Entities.Series series, int episodeCount)
    {
        return new SeriesResponseDto
        {
            Id = series.Id,
            TmdbId = series.TmdbId,
            Title = series.Title,
            Overview = series.Overview,
            PosterUrl = series.PosterUrl,
            NumberOfSeasons = series.NumberOfSeasons,
            NumberOfEpisodes = episodeCount, 
            Rating = series.Rating,
            Notes = series.Notes,
            AddedAt = series.AddedAt
        };
    }
}