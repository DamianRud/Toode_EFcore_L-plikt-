using Microsoft.EntityFrameworkCore;

namespace Tooded_EFCore;

public class ToodedContext : DbContext
{
    public DbSet<Toode> Toodetabel => Set<Toode>();
    public DbSet<Kasutaja> Kasutajad => Set<Kasutaja>();
    public DbSet<Kategooria> Kategooriatabel => Set<Kategooria>();
    public DbSet<Ost> Ostud => Set<Ost>();
    public DbSet<Osturida> OstuRead => Set<Osturida>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(Kaustad.Juur, "tooded.db")}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Kategooria>().ToTable("Kategooriatabel");
        modelBuilder.Entity<Toode>().ToTable("Toodetabel");

        modelBuilder.Entity<Ost>().ToTable("Ostud");
        modelBuilder.Entity<Osturida>().ToTable("OstuRead");
        modelBuilder.Entity<Osturida>()
            .HasOne(r => r.Ost)
            .WithMany(o => o.Read)
            .HasForeignKey(r => r.OstId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Kategooria>().HasData(
            new Kategooria { Id = 1, Kategooria_nimetus = "Joogid" },
            new Kategooria { Id = 2, Kategooria_nimetus = "Puu- ja köögiviljad" },
            new Kategooria { Id = 3, Kategooria_nimetus = "Maiustused" },
            new Kategooria { Id = 4, Kategooria_nimetus = "Pagaritooted" });
    }
}
