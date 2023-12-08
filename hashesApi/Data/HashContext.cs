using Microsoft.EntityFrameworkCore;
using hashesApi.Models;
public class HashContext : DbContext
{
    public HashContext()
    {
    }
    public HashContext(DbContextOptions<HashContext> options) : base(options)
    {
    }

    public DbSet<Hash> Hashes { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hash>()
            .HasKey(h => h.Id);
    }


}