using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Movies.Commands.MarkAsWatched;

public class MarkAsWatchedCommandHandler : IRequestHandler<MarkAsWatchedCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MarkAsWatchedCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(MarkAsWatchedCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<bool>.FailureResult("User not authenticated");
        }

        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == request.MovieId && m.UserId == userId, cancellationToken);

        if (movie == null)
        {
            return ApiResponse<bool>.FailureResult("Movie not found");
        }

        movie.IsWatched = request.IsWatched;
        movie.WatchedAt = request.IsWatched ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync(cancellationToken);

        var message = request.IsWatched ? "Movie marked as watched" : "Movie marked as unwatched";
        return ApiResponse<bool>.SuccessResult(true, message);
    }
}