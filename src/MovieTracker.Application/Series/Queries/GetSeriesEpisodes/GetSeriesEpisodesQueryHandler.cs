using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Queries.GetSeriesEpisodes;

public class GetSeriesEpisodesQueryHandler : IRequestHandler<GetSeriesEpisodesQuery, ApiResponse<SeriesEpisodesResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetSeriesEpisodesQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<SeriesEpisodesResponseDto>> Handle(GetSeriesEpisodesQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<SeriesEpisodesResponseDto>.FailureResult("User not authenticated");
        }

        var series = await _context.Series
            .FirstOrDefaultAsync(s => s.Id == request.SeriesId && s.UserId == userId, cancellationToken);

        if (series == null)
        {
            return ApiResponse<SeriesEpisodesResponseDto>.FailureResult("Series not found");
        }

        var episodes = await _context.Episodes
            .Where(e => e.SeriesId == request.SeriesId)
            .OrderBy(e => e.SeasonNumber)
            .ThenBy(e => e.EpisodeNumber)
            .Select(e => new EpisodeResponseDto
            {
                Id = e.Id,
                SeasonNumber = e.SeasonNumber,
                EpisodeNumber = e.EpisodeNumber,
                Title = e.Title,
                Overview = e.Overview ?? string.Empty,
                AirDate = e.AirDate,
                IsWatched = e.IsWatched,
                WatchedAt = e.WatchedAt
            })
            .ToListAsync(cancellationToken);

        var response = new SeriesEpisodesResponseDto
        {
            SeriesId = series.Id,
            Title = series.Title,
            TotalEpisodes = episodes.Count,
            WatchedEpisodes = episodes.Count(e => e.IsWatched),
            Episodes = episodes
        };

        return ApiResponse<SeriesEpisodesResponseDto>.SuccessResult(response);
    }
}