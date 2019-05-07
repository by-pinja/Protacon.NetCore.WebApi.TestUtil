using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class MockDepenciesTests
    {
        [Fact]
        public async Task WhenHeadersAreDefined_ThenPassThemToApi()
        {
            var host = TestHost.Run<TestStartup>();

            // See TestStartup for information what is done before this.
           host.Setup<IExternalDepency>(x => x.SomeCall(Arg.Is("abc")).Returns("3"));

            await host.Get("/external/abc")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.Value.Should().Be("3"));

            host.Passing<IExternalDepency>(x => x.Received(1).SomeCall("abc"));
        }
    }
}
