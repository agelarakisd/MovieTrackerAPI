using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Commands.AddSeriesNotes;

public class AddSeriesNotesCommandHandler : IRequestHandler<AddSeriesNotesCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddSeriesNotesCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(AddSeriesNotesCommand request, CancellationToken cancellationToken)
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

        var series = await _context.Series
            .FirstOrDefaultAsync(s => s.Id == request.SeriesId && s.UserId == userId, cancellationToken);

        if (series == null)
        {
            return ApiResponse<bool>.FailureResult("Series not found");
        }

        series.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResult(true, "Notes updated successfully");
    }
}