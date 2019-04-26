using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Protacon.NetCore.WebApi.TestUtil.Extensions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class WaitForStatusCodeTests
    {
        [Fact]
        public async Task WhenErronousCodeIsReturned_ThenThrowErrorAfterTimeout()
        {
            // await TestHost.Run<TestStartup>().Get("/returnthree/")
            //     .Invoking(x => x.WaitForStatusCode(HttpStatusCode.BadRequest, TimeSpan.FromSeconds(2)))
            //     .Should().Throw<ExpectedStatusCodeException>();
        }

        [Fact]
        public void WhenValidCodeIsReturned_ThenReturnWithoutError()
        {
            // await TestHost.Run<TestStartup>().Get("/returnthree/")
            //     .Invoking(x => x.WaitForStatusCode(HttpStatusCode.OK, TimeSpan.FromSeconds(2)))
            //     .Should().NotThrow<Exception>();
        }
    }
}