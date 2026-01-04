using Microsoft.EntityFrameworkCore;
using MovieTracker.Domain.Entities;

namespace MovieTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Movie> Movies { get; }
    DbSet<Domain.Entities.Series> Series { get; }
    DbSet<Episode> Episodes { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}