using System;
using System.Net;
using System.Threading;

namespace Protacon.NetCore.WebApi.TestUtil.Extensions
{
    public static class CallResponseExtensions
    {
        public static CallResponse WaitForStatusCode(this CallResponse response, HttpStatusCode statusCode, TimeSpan timeout)
        {
            DateTime endAt = DateTime.UtcNow.Add(timeout);

            while (true)
            {
                try
                {
                    return response.ExpectStatusCode(statusCode);
                }
                catch (ExpectedStatusCodeException ex)
                {
                    if (DateTime.UtcNow > endAt)
                    {
                        throw ex;
                    }

                    Thread.Sleep(500);
                }
            }
        }
    }
}