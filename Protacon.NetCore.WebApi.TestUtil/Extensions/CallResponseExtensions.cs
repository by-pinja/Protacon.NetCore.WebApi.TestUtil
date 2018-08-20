using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil.Extensions
{
    public static class CallResponseExtensions
    {
        public static Call WaitForStatusCode(this Call call, HttpStatusCode statusCode, TimeSpan timeout)
        {
            const int testPeriodMs = 500;
            var timeUsedMs = 0;

            while(true)
            {
                try
                {
                    return call.Clone().ExpectStatusCode(statusCode);
                }
                catch (ExpectedStatusCodeException ex)
                {
                    if (timeUsedMs > timeout.TotalMilliseconds)
                    {
                        throw ex;
                    }

                    timeUsedMs += testPeriodMs;
                    Task.Delay(testPeriodMs).Wait();
                }
            }
        }
    }
}