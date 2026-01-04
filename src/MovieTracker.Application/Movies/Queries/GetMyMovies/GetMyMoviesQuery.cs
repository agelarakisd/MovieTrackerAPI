using MediatR;
using MovieTracker.Application.Common.Models;
using MovieTracker.Application.Movies.Commands.AddMovie;

namespace MovieTracker.Application.Movies.Queries.GetMyMovies;

public class GetMyMoviesQuery : IRequest<ApiResponse<List<MovieResponseDto>>>
{
    public bool? IsWatched { get; set; } 
}