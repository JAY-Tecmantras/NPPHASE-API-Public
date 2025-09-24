using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NPPHASE.Data.Context;

namespace NPPHASE.Data.Factory
{
    public class NPPHASEApiDbContextFactory : IDesignTimeDbContextFactory<NPPHASEApiDbContext>
    {
        public NPPHASEApiDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "NPPHASE.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("NpphaseManagementDB");

            var optionsBuilder = new DbContextOptionsBuilder<NPPHASEApiDbContext>();
            //optionsBuilder.UseSqlServer(connectionString);

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

            optionsBuilder.UseMySql(connectionString, serverVersion);


            return new NPPHASEApiDbContext(optionsBuilder.Options,null);
        }
    }
}
