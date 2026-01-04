using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using MovieTracker.Application.Series.Commands.AddSeries;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Queries.GetMySeries;

public class GetMySeriesQueryHandler : IRequestHandler<GetMySeriesQuery, ApiResponse<List<SeriesResponseDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMySeriesQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<List<SeriesResponseDto>>> Handle(GetMySeriesQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<List<SeriesResponseDto>>.FailureResult("User not authenticated");
        }

        var series = await _context.Series
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.AddedAt)
            .Select(s => new SeriesResponseDto
            {
                Id = s.Id,
                TmdbId = s.TmdbId,
                Title = s.Title,
                Overview = s.Overview,
                PosterUrl = s.PosterUrl,
                NumberOfSeasons = s.NumberOfSeasons,
                NumberOfEpisodes = s.NumberOfEpisodes,
                Rating = s.Rating,
                Notes = s.Notes,
                AddedAt = s.AddedAt,
                FirstAirYear = s.FirstAirDate.HasValue ? s.FirstAirDate.Value.Year : (int?)null
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<SeriesResponseDto>>.SuccessResult(series);
    }
}