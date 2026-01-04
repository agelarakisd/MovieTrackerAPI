using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Movies.Commands.MarkAsWatched;

public class MarkAsWatchedCommand : IRequest<ApiResponse<bool>>
{
    public Guid MovieId { get; set; }
    public bool IsWatched { get; set; }
}