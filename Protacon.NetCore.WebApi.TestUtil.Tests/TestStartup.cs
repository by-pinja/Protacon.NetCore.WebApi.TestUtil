using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class TestStartup
    {
        public TestStartup(IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(Substitute.For<IExternalDepency>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();
            app.UseMvc();
        }
    }
}
