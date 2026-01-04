using MediatR;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Search.Queries.SearchMovies;

public class SearchMoviesQueryHandler : IRequestHandler<SearchMoviesQuery, ApiResponse<List<TmdbMovieDto>>>
{
    private readonly ITmdbService _tmdbService;

    public SearchMoviesQueryHandler(ITmdbService tmdbService)
    {
        _tmdbService = tmdbService;
    }

    public async Task<ApiResponse<List<TmdbMovieDto>>> Handle(SearchMoviesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return ApiResponse<List<TmdbMovieDto>>.FailureResult("Search query cannot be empty");
        }

        var results = await _tmdbService.SearchMoviesAsync(request.Query, cancellationToken);

        return ApiResponse<List<TmdbMovieDto>>.SuccessResult(results);
    }
}