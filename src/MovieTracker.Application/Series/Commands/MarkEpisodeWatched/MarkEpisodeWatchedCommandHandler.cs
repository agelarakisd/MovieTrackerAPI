using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Commands.MarkEpisodeWatched;

public class MarkEpisodeWatchedCommandHandler : IRequestHandler<MarkEpisodeWatchedCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MarkEpisodeWatchedCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(MarkEpisodeWatchedCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<bool>.FailureResult("User not authenticated");
        }

        var episode = await _context.Episodes
            .Include(e => e.Series)
            .FirstOrDefaultAsync(e => e.Id == request.EpisodeId, cancellationToken);

        if (episode == null)
        {
            return ApiResponse<bool>.FailureResult("Episode not found");
        }

        if (episode.Series.UserId != userId)
        {
            return ApiResponse<bool>.FailureResult("Unauthorized access to this episode");
        }

        if (request.IsWatched)
        {
            // SMART MARKING: Mark this episode AND all previous episodes as watched
            var episodesToMark = await _context.Episodes
                .Where(e => e.SeriesId == episode.SeriesId &&
                           (e.SeasonNumber < episode.SeasonNumber ||
                            (e.SeasonNumber == episode.SeasonNumber && e.EpisodeNumber <= episode.EpisodeNumber)))
                .ToListAsync(cancellationToken);

            foreach (var ep in episodesToMark)
            {
                ep.IsWatched = true;
                ep.WatchedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true, $"Marked {episodesToMark.Count} episodes as watched (including previous episodes)");
        }
        else
        {
            // UNMARK: Only unmark THIS specific episode
            episode.IsWatched = false;
            episode.WatchedAt = null;

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true, "Episode unmarked as unwatched");
        }
    }
}