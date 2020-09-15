using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(SPM.Web.Areas.Identity.IdentityHostingStartup))]
namespace SPM.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}