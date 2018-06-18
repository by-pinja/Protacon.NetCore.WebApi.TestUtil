using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class HeaderSupportTest
    {
        [Fact]
        public void WhenHeadersAreDefined_ThenPassThemToApi()
        {
            TestHost.Run<TestStartup>().Get("/headertest/",
                    headers: new Dictionary<string, string> {{"example", "somevalue"}})
                .ExpectStatusCode(HttpStatusCode.NoContent);

            TestHost.Run<TestStartup>().Invoking(x => x.Get("/headertest/",
                    headers: new Dictionary<string, string> {{"somethingElse", "somevalue"}}))
                .Should().Throw<InvalidOperationException>();
        }
    }
}
