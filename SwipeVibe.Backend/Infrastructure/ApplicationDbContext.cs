using Microsoft.EntityFrameworkCore;
using SwipeVibe.Backend.Models.Database;
using SwipeVibe.Backend.Models.Profile;
using SwipeVibe.Backend.Models.User;

namespace SwipeVibe.Backend.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<UserModelDb> Users { get; set; }
    public DbSet<ProfileModelDb> Profiles { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModelDb>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<ProfileModelDb>(p => p.UserId);

        modelBuilder.Entity<UserModelDb>()
            .HasIndex(u => u.Msisdn)
            .IsUnique();

        modelBuilder.Entity<UserModelDb>()
            .HasKey(k => k.UserId);

        modelBuilder.Entity<ProfileModelDb>()
            .HasKey(k => k.ProfileId);
    }
}