namespace MovieTracker.Domain.Entities;

public class Episode
{
    public Guid Id { get; set; }
    public Guid SeriesId { get; set; }
    public int TmdbEpisodeId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public DateTime? AirDate { get; set; }
    public bool IsWatched { get; set; }
    public DateTime? WatchedAt { get; set; }
    
    public Series Series { get; set; } = null!;
}