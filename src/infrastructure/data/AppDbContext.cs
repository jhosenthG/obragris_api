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

        // Configuración explícita por entidad (AOT-Safe)
        ConfigureEntity<User>(modelBuilder);
        ConfigureEntity<Project>(modelBuilder);
        ConfigureEntity<Report>(modelBuilder);
        ConfigureEntity<Task>(modelBuilder);
    }

    private static void ConfigureEntity<T>(ModelBuilder modelBuilder) where T : BaseEntity
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()"); // Postgres

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });
    }
}