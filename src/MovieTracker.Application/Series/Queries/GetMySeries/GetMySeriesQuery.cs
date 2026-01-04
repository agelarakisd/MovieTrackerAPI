using MediatR;
using MovieTracker.Application.Common.Models;
using MovieTracker.Application.Series.Commands.AddSeries;

namespace MovieTracker.Application.Series.Queries.GetMySeries;

public class GetMySeriesQuery : IRequest<ApiResponse<List<SeriesResponseDto>>>
{
}