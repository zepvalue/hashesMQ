using Microsoft.EntityFrameworkCore;
public class HashContext : DbContext
{
    
    public DbSet<Hash>? Hashes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure your database connection here
        optionsBuilder.UseSqlServer("LocalDbConnStr");
    }
}

public class Hash
{
    public int Id { get; set; }
    public string? Date { get; set; }
    public string? Sha1 { get; set; }
}
