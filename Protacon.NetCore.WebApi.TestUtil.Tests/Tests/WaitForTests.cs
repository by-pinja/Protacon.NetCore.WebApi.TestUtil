using System;
using FluentAssertions;
using Protacon.NetCore.WebApi.TestUtil.Extensions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class WaitForTests
    {
        [Fact]
        public void WhenNoValidResponseIsReceived_ThenThrowErrorAfterTimeout()
        {
            TestHost.Run<TestStartup>().Get("/returnthree/")
                .Awaiting(x => x.WaitFor<int>(r => r.Should().Be(1), TimeSpan.FromSeconds(2)))
                .Should().Throw<Exception>();
        }

        [Fact]
        public void WhenValidResponseIsReceived_ThenReturnWithoutError()
        {
            TestHost.Run<TestStartup>().Get("/returnthree/")
                .Awaiting(x => x.WaitFor<int>(r => r.Should().Be(3), TimeSpan.FromSeconds(2)))
                .Should().NotThrow<Exception>();
        }
    }
}