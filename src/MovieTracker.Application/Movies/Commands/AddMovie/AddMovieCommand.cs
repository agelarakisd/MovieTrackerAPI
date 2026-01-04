using MediatR;
using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Movies.Commands.AddMovie;

public class AddMovieCommand : IRequest<ApiResponse<MovieResponseDto>>
{
    public int TmdbId { get; set; }
}

public class MovieResponseDto
{
    public Guid Id { get; set; }
    public int TmdbId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int? ReleaseYear { get; set; }
    public bool IsWatched { get; set; }
    public decimal? Rating { get; set; }
    public string? Notes { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? WatchedAt { get; set; }
}