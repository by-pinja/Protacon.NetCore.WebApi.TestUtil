using System;
using System.Threading.Tasks;
using FluentAssertions;
using Protacon.NetCore.WebApi.TestUtil.Extensions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class WaitForTests
    {
        [Fact]
        public async Task WhenNoValidResponseIsReceived_ThenThrowErrorAfterTimeout()
        {
            await TestHost.Run<TestStartup>().Get("/returnthree/")
                .Awaiting(x => x.WaitFor<int>(r => r.Should().Be(1), TimeSpan.FromSeconds(2)))
                .Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task WhenValidResponseIsReceived_ThenReturnWithoutError()
        {
            await TestHost.Run<TestStartup>().Get("/returnthree/")
                .Awaiting(x => x.WaitFor<int>(r => r.Should().Be(3), TimeSpan.FromSeconds(2)))
                .Should().NotThrowAsync<Exception>();
        }
    }
}