using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Movies.Commands.RateMovie;

public class RateMovieCommandHandler : IRequestHandler<RateMovieCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RateMovieCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(RateMovieCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating < 1 || request.Rating > 10)
        {
            return ApiResponse<bool>.FailureResult("Rating must be between 1 and 10");
        }

        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<bool>.FailureResult("User not authenticated");
        }

        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == request.MovieId && m.UserId == userId, cancellationToken);

        if (movie == null)
        {
            return ApiResponse<bool>.FailureResult("Movie not found");
        }

        movie.Rating = request.Rating;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResult(true, "Movie rated successfully");
    }
}