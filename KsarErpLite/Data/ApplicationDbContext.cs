namespace KsarErpLite.Data;

using Microsoft.EntityFrameworkCore;
using KsarErpLite.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<WorkType> WorkTypes { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Act> Acts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Подключение расширения для работы с пространственными данными PostGIS
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();

        modelBuilder.Entity<Job>()
            .HasOne(j => j.CreatedBy)
            .WithMany(u => u.CreatedJobs)
            .HasForeignKey(j => j.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Foreman)
            .WithMany(u => u.AssignedJobs)
            .HasForeignKey(j => j.ForemanId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Act>()
            .HasIndex(a => a.JobId)
            .IsUnique();
    }
}