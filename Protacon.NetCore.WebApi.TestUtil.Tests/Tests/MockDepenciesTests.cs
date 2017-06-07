using System.Net;
using FluentAssertions;
using NSubstitute;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class MockDepenciesTests
    {
        [Fact]
        public void WhenHeadersAreDefined_ThenPassThemToApi()
        {
            var host = TestHost.Run<TestStartup>();

            // See TestStartup for information what is done before this.
           host.MockSetup<IExternalDepency>(x => x.SomeCall(Arg.Is("abc")).Returns("3"));

            host.Get("/external/abc")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.Value.Should().Be("3"));

            host.MockPassing<IExternalDepency>(x => x.Received(1).SomeCall("abc"));
        }
    }
}
