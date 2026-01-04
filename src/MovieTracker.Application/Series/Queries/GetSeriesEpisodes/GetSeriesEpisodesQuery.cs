using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Series.Queries.GetSeriesEpisodes;

public class GetSeriesEpisodesQuery : IRequest<ApiResponse<SeriesEpisodesResponseDto>>
{
    public Guid SeriesId { get; set; }
}

public class SeriesEpisodesResponseDto
{
    public Guid SeriesId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalEpisodes { get; set; }
    public int WatchedEpisodes { get; set; }
    public List<EpisodeResponseDto> Episodes { get; set; } = new();
}

public class EpisodeResponseDto
{
    public Guid Id { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public DateTime? AirDate { get; set; }
    public bool IsWatched { get; set; }
    public DateTime? WatchedAt { get; set; }
}