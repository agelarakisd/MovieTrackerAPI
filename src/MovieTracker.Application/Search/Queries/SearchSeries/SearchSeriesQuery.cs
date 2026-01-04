using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Search.Queries.SearchSeries;

public class SearchSeriesQuery : IRequest<ApiResponse<List<TmdbSeriesDto>>>
{
    public string Query { get; set; } = string.Empty;
}