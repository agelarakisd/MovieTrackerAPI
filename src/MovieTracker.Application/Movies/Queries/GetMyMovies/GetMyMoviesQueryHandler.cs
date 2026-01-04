using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using MovieTracker.Application.Movies.Commands.AddMovie;
using System.Security.Claims;

namespace MovieTracker.Application.Movies.Queries.GetMyMovies;

public class GetMyMoviesQueryHandler : IRequestHandler<GetMyMoviesQuery, ApiResponse<List<MovieResponseDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyMoviesQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<List<MovieResponseDto>>> Handle(GetMyMoviesQuery request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<List<MovieResponseDto>>.FailureResult("User not authenticated");
        }

        var query = _context.Movies.Where(m => m.UserId == userId);

        if (request.IsWatched.HasValue)
        {
            query = query.Where(m => m.IsWatched == request.IsWatched.Value);
        }

        var movies = await query
            .OrderByDescending(m => m.AddedAt)
            .Select(m => new MovieResponseDto
            {
                Id = m.Id,
                TmdbId = m.TmdbId,
                Title = m.Title,
                Overview = m.Overview,
                PosterUrl = m.PosterUrl,
                ReleaseYear = m.ReleaseYear,
                IsWatched = m.IsWatched,
                Rating = m.Rating,
                Notes = m.Notes,
                AddedAt = m.AddedAt,
                WatchedAt = m.WatchedAt
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MovieResponseDto>>.SuccessResult(movies);
    }
}