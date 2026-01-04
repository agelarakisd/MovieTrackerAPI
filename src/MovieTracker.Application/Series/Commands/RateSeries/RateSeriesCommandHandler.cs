using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MovieTracker.Application.Common.Interfaces;
using MovieTracker.Application.Common.Models;
using System.Security.Claims;

namespace MovieTracker.Application.Series.Commands.RateSeries;

public class RateSeriesCommandHandler : IRequestHandler<RateSeriesCommand, ApiResponse<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RateSeriesCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<bool>> Handle(RateSeriesCommand request, CancellationToken cancellationToken)
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

        var series = await _context.Series
            .FirstOrDefaultAsync(s => s.Id == request.SeriesId && s.UserId == userId, cancellationToken);

        if (series == null)
        {
            return ApiResponse<bool>.FailureResult("Series not found");
        }

        series.Rating = request.Rating;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResult(true, "Series rated successfully");
    }
}