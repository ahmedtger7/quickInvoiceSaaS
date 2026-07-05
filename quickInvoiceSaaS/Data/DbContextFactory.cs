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

     var connectionString = "Host=aws-0-eu-west-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.wgthqndvxujqvvtdkojq;Password=Ekthetiger8579;SSL Mode=Require;Trust Server Certificate=true";

            optionsBuilder.UseNpgsql(connectionString);

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
