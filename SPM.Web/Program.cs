using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SPM.Web.Data;

namespace SPM.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Build the host
            var host = CreateHostBuilder(args).Build();

            // Get the database service from the host
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Seed database with defautl data
            SeedData.SeedSettings(context);

            // Run host
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
