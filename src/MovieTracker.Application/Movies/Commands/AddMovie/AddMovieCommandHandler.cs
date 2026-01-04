using MediatR;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using MovieTracker.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MovieTracker.Application.Movies.Commands.AddMovie;

public class AddMovieCommandHandler : IRequestHandler<AddMovieCommand, ApiResponse<MovieResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITmdbService _tmdbService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddMovieCommandHandler(
        IApplicationDbContext context, 
        ITmdbService tmdbService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _tmdbService = tmdbService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<MovieResponseDto>> Handle(AddMovieCommand request, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<MovieResponseDto>.FailureResult("User not authenticated");
        }

        // Check if movie already exists in user's list (including soft deleted)
        var existingMovie = await _context.Movies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.UserId == userId && m.TmdbId == request.TmdbId, cancellationToken);

        if (existingMovie != null)
        {
            // If soft deleted, restore it
            if (existingMovie.IsDeleted)
            {
                existingMovie.IsDeleted = false;
                existingMovie.DeletedAt = null;
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<MovieResponseDto>.SuccessResult(MapToDto(existingMovie), "Movie restored to your list");
            }

            return ApiResponse<MovieResponseDto>.FailureResult("Movie already in your list");
        }

        var movieDetails = await _tmdbService.GetMovieDetailsAsync(request.TmdbId, cancellationToken);
        if (movieDetails == null)
        {
            return ApiResponse<MovieResponseDto>.FailureResult("Movie not found in TMDB");
        }

        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TmdbId = request.TmdbId,
            Title = movieDetails.Title,
            Overview = movieDetails.Overview,
            PosterUrl = movieDetails.PosterPath != null ? $"https://image.tmdb.org/t/p/w500{movieDetails.PosterPath}" : null,
            BackdropUrl = movieDetails.BackdropPath != null ? $"https://image.tmdb.org/t/p/w500{movieDetails.BackdropPath}" : null,
            ReleaseYear = movieDetails.Year,
            Genre = movieDetails.Genres.Any() ? string.Join(", ", movieDetails.Genres.Select(g => g.Name)) : null,
            Runtime = movieDetails.Runtime,
            IsWatched = false,
            AddedAt = DateTime.UtcNow
        };

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<MovieResponseDto>.SuccessResult(MapToDto(movie), "Movie added to your list");
    }

    private static MovieResponseDto MapToDto(Movie movie)
    {
        return new MovieResponseDto
        {
            Id = movie.Id,
            TmdbId = movie.TmdbId,
            Title = movie.Title,
            Overview = movie.Overview,
            PosterUrl = movie.PosterUrl,
            ReleaseYear = movie.ReleaseYear,
            IsWatched = movie.IsWatched,
            Rating = movie.Rating,
            Notes = movie.Notes,
            AddedAt = movie.AddedAt
        };
    }
}