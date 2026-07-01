using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QuickInvoiceSaaS.Data;
using QuickInvoiceSaaS.Services;

namespace QuickInvoiceSaaS.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = "Server=DESKTOP-7O20K25;Database=QuickInvoiceSaaS_Db;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

    public class DummyTenantService : ITenantService
    {
        public string GetTenantId()
        {
            return string.Empty;
        }
    }
}