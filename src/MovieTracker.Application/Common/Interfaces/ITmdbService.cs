using MovieTracker.Application.Common.Models;

namespace MovieTracker.Application.Common.Interfaces;

public interface ITmdbService
{
    Task<List<TmdbMovieDto>> SearchMoviesAsync(string query, CancellationToken cancellationToken = default);
    Task<TmdbMovieDetailsDto?> GetMovieDetailsAsync(int tmdbId, CancellationToken cancellationToken = default);
    Task<List<TmdbSeriesDto>> SearchSeriesAsync(string query, CancellationToken cancellationToken = default);
    Task<TmdbSeriesDetailsDto?> GetSeriesDetailsAsync(int tmdbId, CancellationToken cancellationToken = default);
}