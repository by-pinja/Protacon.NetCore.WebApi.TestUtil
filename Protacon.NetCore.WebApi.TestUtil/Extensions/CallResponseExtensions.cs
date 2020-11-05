using System;
using System.Net;
using System.Threading.Tasks;

namespace Protacon.NetCore.WebApi.TestUtil.Extensions
{
    public static class CallResponseExtensions
    {
        public static async Task<Call> WaitForStatusCode(this Task<Call> call, HttpStatusCode statusCode, TimeSpan timeout)
        {
            const int testPeriodMs = 100;
            var timeUsedMs = 0;

            while (true)
            {
                try
                {
                    return await (await call.ConfigureAwait(false)).Clone().ExpectStatusCode(statusCode).ConfigureAwait(false);
                }
                catch (ExpectedStatusCodeException ex)
                {
                    timeUsedMs = WaitTimeHandler(timeout, testPeriodMs, timeUsedMs, ex);
                }
            }
        }

        public static async Task<Call> WaitFor<T>(
            this Task<Call> call,
            Action<T> assertionToFulfill,
            TimeSpan timeout,
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            const int testPeriodMs = 100;
            var timeUsedMs = 0;

            while (true)
            {
                try
                {
                    var clonedCall = (await call.ConfigureAwait(false))
                        .Clone()
                        .ExpectStatusCode(expectedStatusCode);
                    
                    await clonedCall
                        .WithContentOf<T>()
                        .Passing(assertionToFulfill)
                        .ConfigureAwait(false);

                    return await clonedCall;
                }
                catch (Exception ex)
                {
                    timeUsedMs = WaitTimeHandler(timeout, testPeriodMs, timeUsedMs, ex);
                }
            }
        }

        private static int WaitTimeHandler(TimeSpan timeout, int testPeriodMs, int timeUsedMs, Exception ex)
        {
            if (timeUsedMs > timeout.TotalMilliseconds)
            {
                throw ex;
            }

            timeUsedMs += testPeriodMs;
            Task.Delay(testPeriodMs).Wait();
            return timeUsedMs;
        }
    }
}