using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace TwitterBook.Installer
{
    public class MvcInstaller : IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "TweetBook API", Version = "v1" });
            });
        }
    }
}