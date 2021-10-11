using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TwitterBook.Installer
{
    public interface IInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration);
    }
}