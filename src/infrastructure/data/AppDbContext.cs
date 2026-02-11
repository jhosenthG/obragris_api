using Microsoft.EntityFrameworkCore;
using obragris_api.core;
using Task = obragris_api.core.Task;

namespace obragris_api.infrastructure.data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Task> ProjectTasks => Set<Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureProject(modelBuilder);
        ConfigureReport(modelBuilder);
        ConfigureTask(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });
    }

    private static void ConfigureProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });
    }

    private static void ConfigureReport(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });
    }

    private static void ConfigureTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });
    }
}