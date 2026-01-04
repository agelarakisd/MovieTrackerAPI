using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieTracker.Domain.Entities;

namespace MovieTracker.Infrastructure.Data.Configurations;

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Overview)
            .HasMaxLength(1000);

        builder.HasIndex(e => new { e.SeriesId, e.SeasonNumber, e.EpisodeNumber })
            .IsUnique();

        builder.HasIndex(e => e.TmdbEpisodeId);
    }
}