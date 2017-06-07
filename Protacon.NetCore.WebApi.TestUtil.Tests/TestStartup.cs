using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class TestStartup
    {
        private readonly Startup _original;

        public TestStartup(IHostingEnvironment env)
        {
            _original = new Startup(env);
        }


        public void ConfigureServices(IServiceCollection services)
        {
            _original.ConfigureServices(services);

            services.RemoveService<IExternalDepency>()
                .AddSingleton(Substitute.For<IExternalDepency>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _original.Configure(app, env, loggerFactory);
        }
    }
}
