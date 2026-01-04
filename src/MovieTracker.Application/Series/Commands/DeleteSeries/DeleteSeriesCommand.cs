using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Series.Commands.DeleteSeries;

public class DeleteSeriesCommand : IRequest<ApiResponse<bool>>
{
    public Guid SeriesId { get; set; }
}