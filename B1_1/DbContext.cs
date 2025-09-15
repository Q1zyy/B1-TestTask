using Microsoft.EntityFrameworkCore;

namespace B1_1;

public class RecordsContext : DbContext
{
    private readonly string _connectionString = "Host=localhost;Port=5432;Database=b1;Username=postgres;Password=qazzy";

    public DbSet<Record> Records { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Record>(entity =>
        {
            entity.ToTable("records");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateField).HasColumnType("date").HasColumnName("date_field");
            entity.Property(e => e.EnglishString).HasColumnName("english_string").HasMaxLength(10);
            entity.Property(e => e.RussinaString).HasColumnName("russian_string").HasMaxLength(10);
            entity.Property(e => e.PositiveEvenInteger).HasColumnName("positive_integer_value");
            entity.Property(e => e.DoubleValue).HasColumnName("double_value").HasColumnType("numeric(20,8)");
        });
    }
}
