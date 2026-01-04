using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Commands.DeleteSeries;

public class DeleteSeriesCommandHandler : IRequestHandler<DeleteSeriesCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteSeriesCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
    {
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

        // Soft delete
        series.IsDeleted = true;
        series.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResult(true, "Series removed from your list");
    }
}