using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class TestStartup
    {
        public TestStartup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(Substitute.For<IExternalDepency>());
        }

        public void Configure(IApplicationBuilder app)
        {
#if NETCOREAPP2_1
            app.UseMvc();
#else
            app.UseRouting();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
#endif
        }
    }
}
