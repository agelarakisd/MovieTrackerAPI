using MediatR;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Search.Queries.SearchSeries;

public class SearchSeriesQueryHandler : IRequestHandler<SearchSeriesQuery, ApiResponse<List<TmdbSeriesDto>>>
{
    private readonly ITmdbService _tmdbService;

    public SearchSeriesQueryHandler(ITmdbService tmdbService)
    {
        _tmdbService = tmdbService;
    }

    public async Task<ApiResponse<List<TmdbSeriesDto>>> Handle(SearchSeriesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return ApiResponse<List<TmdbSeriesDto>>.FailureResult("Search query cannot be empty");
        }

        var results = await _tmdbService.SearchSeriesAsync(request.Query, cancellationToken);

        return ApiResponse<List<TmdbSeriesDto>>.SuccessResult(results);
    }
}