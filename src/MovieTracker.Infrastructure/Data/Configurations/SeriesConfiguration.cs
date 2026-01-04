using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieTracker.Domain.Entities;

namespace MovieTracker.Infrastructure.Data.Configurations;

public class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Overview)
            .HasMaxLength(2000);

        builder.Property(s => s.PosterUrl)
            .HasMaxLength(500);

        builder.Property(s => s.BackdropUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Genre)
            .HasMaxLength(200);

        builder.Property(s => s.Rating)
            .HasPrecision(3, 1);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(s => s.TmdbId);
        builder.HasIndex(s => new { s.UserId, s.IsDeleted });

        builder.HasMany(s => s.Episodes)
            .WithOne(e => e.Series)
            .HasForeignKey(e => e.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}