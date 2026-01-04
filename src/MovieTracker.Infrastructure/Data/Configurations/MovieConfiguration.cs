using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieTracker.Domain.Entities;

namespace MovieTracker.Infrastructure.Data.Configurations;

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Overview)
            .HasMaxLength(2000);

        builder.Property(m => m.PosterUrl)
            .HasMaxLength(500);

        builder.Property(m => m.BackdropUrl)
            .HasMaxLength(500);

        builder.Property(m => m.Genre)
            .HasMaxLength(200);

        builder.Property(m => m.Rating)
            .HasPrecision(3, 1);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(m => m.TmdbId);
        builder.HasIndex(m => new { m.UserId, m.IsDeleted });

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}