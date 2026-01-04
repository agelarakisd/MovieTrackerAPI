using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Series.Commands.AddSeries;

public class AddSeriesCommand : IRequest<ApiResponse<SeriesResponseDto>>
{
    public int TmdbId { get; set; }
}

public class SeriesResponseDto
{
    public Guid Id { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int NumberOfSeasons { get; set; }
    public int NumberOfEpisodes { get; set; }
    public decimal? Rating { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
    public int? FirstAirYear { get; set; }
}