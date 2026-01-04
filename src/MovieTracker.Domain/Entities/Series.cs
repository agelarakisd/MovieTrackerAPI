namespace MovieTracker.Domain.Entities;

public class Series
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public string? BackdropUrl { get; set; }
    public DateTime? FirstAirDate { get; set; }
    public string? Genre { get; set; }
    public int NumberOfSeasons { get; set; }
    public int NumberOfEpisodes { get; set; }
    public decimal? Rating { get; set; } // 1-10
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
    public bool IsDeleted { get; set; } // Soft delete
    public DateTime? DeletedAt { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
}