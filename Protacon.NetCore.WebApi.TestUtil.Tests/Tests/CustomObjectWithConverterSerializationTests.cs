using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Protacon.NetCore.WebApi.TestUtil.Tests.Dummy;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class CustomObjectWithConverterSerializationTests
    {
        [Fact]
        public async Task WhenCustomObjectIsRequested_ThenItCanBeParsedProperlyWithCustomConverterInBothEnds()
        {
            await TestHost.Run<TestStartup>().Get("/returnobject/")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.AnotherValue.Content.Should().Be("default_value"));

            await TestHost.Run<TestStartup>().Put("/returnsame/", new DummyRequest { AnotherValue = new CustomTestObject("valuehere") })
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.AnotherValue.Content.Should().Be("valuehere"));

            await TestHost.Run<TestStartup>().Post("/returnsame/", new DummyRequest { AnotherValue = new CustomTestObject("valuehere") })
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<DummyRequest>()
                .Passing(x => x.AnotherValue.Content.Should().Be("valuehere"));
        }
    }
}