namespace B1_2.Infrastructure;

using B1_2.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UploadedFileEntity> UploadedFiles { get; set; }
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<AccountClassEntity> AccountClasses { get; set; }
    public DbSet<BankEntity> Banks { get; set; }
}
