using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Series.Commands.AddSeriesNotes;

public class AddSeriesNotesCommand : IRequest<ApiResponse<bool>>
{
    public Guid SeriesId { get; set; }
    public string? Notes { get; set; }
}