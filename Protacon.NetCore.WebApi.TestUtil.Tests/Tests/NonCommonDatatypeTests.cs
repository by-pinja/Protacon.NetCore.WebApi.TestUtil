using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Protacon.NetCore.WebApi.TestUtil.Tests.Tests
{
    public class NonCommonDatatypeTests
    {
        [Fact]
        public void WhenFileIsDownloaded_ThenResultsCanBeAsserted()
        {
            TestHost.Run<TestStartup>().Get("/file/")
                .ExpectStatusCode(HttpStatusCode.OK)
                .WithContentOf<Byte[]>()
                .Passing(x => x.Length.Should().Be(4));
        }
    }
}
