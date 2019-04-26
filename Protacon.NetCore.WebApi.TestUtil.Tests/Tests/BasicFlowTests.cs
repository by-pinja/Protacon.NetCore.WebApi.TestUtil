using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class BasicFlowTests
    {
        [Fact]
        public async Task WhenGetIsCalled_ThenAssertingItWorks()
        {
            var foo = await TestHost.Run<TestStartup>().Get("/returnthree/")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<int>();

            await TestHost.Run<TestStartup>().Get("/returnthree/")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<int>()
                .Passing(
                    x => x.Should().Be(3));

            // await TestHost.Run<TestStartup>().Get("/returnthree/")
            //     .Invoking(x => x.ExpectStatusCode(HttpStatusCode.NoContent))
            //     .Should()
            //     .Throw<ExpectedStatusCodeException>();
        }

        [Fact]
        public async Task  WhenDeleteIsCalled_ThenAssertingItWorks()
        {
            await TestHost.Run<TestStartup>().Delete("/something/abc")
                .ExpectStatusCode(HttpStatusCode.NoContent);

            // TestHost.Run<TestStartup>().Delete("/something/abc")
            //     .Invoking(x => x.ExpectStatusCode(HttpStatusCode.NotFound))
            //     .Should().Throw<ExpectedStatusCodeException>();
        }

        [Fact]
        public async Task  WhenPutIsCalled_ThenAssertingItWorks()
        {
            await TestHost.Run<TestStartup>().Put("/returnsame/", new DummyRequest { Value = "3" })
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.Value.Should().Be("3"));

            TestHost.Run<TestStartup>().Put("/returnsame/", new { value = 3 })
                .Invoking(x => x.ExpectStatusCode(HttpStatusCode.NotFound))
                .Should().Throw<ExpectedStatusCodeException>();
        }

        [Fact]
        public async Task  WhenPostIsCalled_ThenAssertingItWorks()
        {
            await TestHost.Run<TestStartup>().Post("/returnsame/", new DummyRequest { Value = "3" })
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.Value.Should().Be("3"));

            // TestHost.Run<TestStartup>().Post("/returnsame/", new { value = 3 })
            //     .Invoking(x => x.ExpectStatusCode(HttpStatusCode.NotFound))
            //     .Should().Throw<ExpectedStatusCodeException>();
        }

        [Fact]
        public async Task  WhenNonAcceptedCodeIsExpected_ThenAcceptItAsResult()
        {
            await TestHost.Run<TestStartup>().Get("/errorcontent/")
                .ExpectStatusCode(HttpStatusCode.NotFound)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.Value.Should().Be("error"));
        }

        [Fact]
        public async Task  WhenExpectedCodeIsNotDefinedOnError_ThenFail()
        {
            // await TestHost.Run<TestStartup>().Get("/errorcontent/")
            //     .Invoking(x => x.WithContentOf<DummyRequest>())
            //     .Should().Throw<ExpectedStatusCodeException>();
        }
    }
}
