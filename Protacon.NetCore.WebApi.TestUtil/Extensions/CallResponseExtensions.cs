using System;
using System.Net;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil.Extensions
{
    public static class CallResponseExtensions
    {
        public static async Task<Call> WaitForStatusCode(this Task<Call> call, HttpStatusCode statusCode, TimeSpan timeout)
        {
            const int testPeriodMs = 500;
            var timeUsedMs = 0;

            while(true)
            {
                try
                {
                    return await (await call).Clone().ExpectStatusCode(statusCode).ConfigureAwait(false);
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