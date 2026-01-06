using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.Api.Modules.Awards.Domain;
using GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure.Entities;

namespace GoldenRaspberryAwards.Api.Modules.Awards.Infrastructure;

public class AwardsDbContext(DbContextOptions<AwardsDbContext> options) : DbContext(options)
{
    public DbSet<ProducerWinEntity> ProducerWins => Set<ProducerWinEntity>();
    public DbSet<ProducerIntervalEntity> ProducerIntervals => Set<ProducerIntervalEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProducerWinEntity>(entity =>
        {
            entity.ToTable("producer_wins");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProducerName).HasColumnName("producer_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Year).HasColumnName("year").IsRequired();
            entity.HasIndex(e => new { e.ProducerName, e.Year }).IsUnique();
        });

        modelBuilder.Entity<ProducerIntervalEntity>(entity =>
        {
            entity.ToTable("producer_intervals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProducerName).HasColumnName("producer_name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.PreviousWin).HasColumnName("previous_win").IsRequired();
            entity.Property(e => e.FollowingWin).HasColumnName("following_win").IsRequired();
            entity.Property(e => e.Interval).HasColumnName("interval").IsRequired();
        });
    }
}
