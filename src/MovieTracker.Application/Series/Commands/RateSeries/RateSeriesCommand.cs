using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Series.Commands.RateSeries;

public class RateSeriesCommand : IRequest<ApiResponse<bool>>
{
    public Guid SeriesId { get; set; }
    public decimal Rating { get; set; } // 1-10
}