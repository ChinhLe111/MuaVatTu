using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MuaVatTu.Data
{
    public class MuaVatTuDbContextFactory : IDesignTimeDbContextFactory<MuaVatTuContext>
    {
        public MuaVatTuContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MuaVatTuContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new MuaVatTuContext(optionsBuilder.Options);
        }
    }
}
