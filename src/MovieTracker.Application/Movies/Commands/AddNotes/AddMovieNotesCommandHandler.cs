using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Movies.Commands.AddNotes;

public class AddMovieNotesCommandHandler : IRequestHandler<AddMovieNotesCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddMovieNotesCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(AddMovieNotesCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Notes) && request.Notes.Length > 1000)
        {
            return ApiResponse<bool>.FailureResult("Notes cannot exceed 1000 characters");
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

        movie.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResult(true, "Notes updated successfully");
    }
}