using System;
using System.Net;
using FluentAssertions;
using Protacon.NetCore.WebApi.TestUtil.Extensions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class WaitForStatusCodeTests
    {
        [Fact]
        public void WhenErronousCodeIsReturned_ThenThrowErrorAfterTimeout()
        {
            TestHost.Run<TestStartup>().Get("/returnthree/")
                .Invoking(x => x.WaitForStatusCode(HttpStatusCode.BadRequest, TimeSpan.FromSeconds(2)))
                .Should().Throw<ExpectedStatusCodeException>();
        }

        [Fact]
        public void WhenValidCodeIsReturned_ThenReturnWithoutError()
        {
            TestHost.Run<TestStartup>().Get("/returnthree/")
                .Invoking(x => x.WaitForStatusCode(HttpStatusCode.OK, TimeSpan.FromSeconds(2)))
                .Should().NotThrow<Exception>();
        }
    }
}