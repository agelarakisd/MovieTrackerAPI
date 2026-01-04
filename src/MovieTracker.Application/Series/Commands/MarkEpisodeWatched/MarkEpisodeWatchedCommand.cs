using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Series.Commands.MarkEpisodeWatched;

public class MarkEpisodeWatchedCommand : IRequest<ApiResponse<bool>>
{
    public Guid EpisodeId { get; set; }
    public bool IsWatched { get; set; }
}