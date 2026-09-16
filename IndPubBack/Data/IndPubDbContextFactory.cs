using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IndPubBack.Data;

public class IndPubDbContextFactory : IDesignTimeDbContextFactory<IndPubDbContext>
{
    public IndPubDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("IndPubConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'IndPubConnection' not found. " +
                                   "Set it in appsettings.json or the IndPubConnection environment variable.");

        var optionsBuilder = new DbContextOptionsBuilder<IndPubDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new IndPubDbContext(optionsBuilder.Options);
    }
}