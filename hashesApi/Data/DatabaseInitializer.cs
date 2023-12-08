using System;
using Microsoft.EntityFrameworkCore;
using hashesApi.Models;

public class DatabaseInitializer

{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Initialize()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<HashContext>();

                if (!context.Database.CanConnect())
                {
                    context.Database.Migrate();
                    SeedInitialData(context);
                    Console.WriteLine("Database created, migrated, and seeded successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred during database initialization: " + ex.Message);
                // Handle the exception accordingly
            }
        }
    }

    private void SeedInitialData(HashContext context)
    {
        // Example: Seed initial data into the database
        var initialEntities = new List<Hash>
        {
            new Hash { Date = new DateTime(2023, 12, 25, 10, 00, 50), Sha1 = "firstHashSha1" },
            new Hash { Date = new DateTime(2023, 12, 25, 10, 30, 50), Sha1 = "secondHashSha1" },
            // Add more entities as needed
        };

        // Add the initial entities to the DbSet and save changes
        context.Hashes.AddRange(initialEntities);
        context.SaveChanges();
    }
}
