using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class HeaderSupportTest
    {
        [Fact]
        public async Task WhenHeadersAreDefined_ThenPassThemToApi()
        {
            await TestHost.Run<TestStartup>().Get("/headertest/",
                    headers: new Dictionary<string, string> {{"example", "somevalue"}})
                .ExpectStatusCode(HttpStatusCode.NoContent);

            TestHost.Run<TestStartup>()
                .Awaiting(x => x.Get("/headertest/",
                    headers: new Dictionary<string, string> {{"somethingElse", "somevalue"}})
                        .ExpectStatusCode(HttpStatusCode.NoContent))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task WhenHeaderRespond_ThenPassThemToHeaderPass()
        {
            await TestHost.Run<TestStartup>().Get("/headertest/response-headers/")
                .ExpectStatusCode(HttpStatusCode.NoContent)
                .HeaderPassing("Content-Language", value => value.Should().Be("fi_FI"))
                .HeaderPassing("X-Powered-By", value => value.Should().Be("pinja"));
        }
    }
}
