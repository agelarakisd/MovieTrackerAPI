using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Movies.Commands.DeleteMovie;

public class DeleteMovieCommand : IRequest<ApiResponse<bool>>
{
    public Guid MovieId { get; set; }
}