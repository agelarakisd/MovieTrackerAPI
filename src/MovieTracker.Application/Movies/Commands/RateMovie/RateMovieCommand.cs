using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Movies.Commands.RateMovie;

public class RateMovieCommand : IRequest<ApiResponse<bool>>
{
    public Guid MovieId { get; set; }
    public decimal Rating { get; set; } // 1-10
}