using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Search.Queries.SearchMovies;

public class SearchMoviesQuery : IRequest<ApiResponse<List<TmdbMovieDto>>>
{
    public string Query { get; set; } = string.Empty;
}