using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class NonCommonDatatypeTests
    {
        [Fact]
        public async Task WhenFileIsDownloaded_ThenResultsCanBeAsserted()
        {
            await TestHost.Run<TestStartup>().Get("/file/")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<Byte[]>()
                .Passing(x => x.Length.Should().Be(4));
        }

        [Fact]
        public async Task WhenHtmlPageIsReturned_ThenResultsCanBeAsserted()
        {
            await TestHost.Run<TestStartup>().Get("/page/")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<string>()
                .Passing(x => x.Should().Contain("Hello World"));
        }
    }
}
